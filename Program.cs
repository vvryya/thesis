﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// Класс, моделирующий поведение агента
class CarAgent
{
    private static readonly Random rand = new Random();

    // Константы наград и штрафов   
    public const float GoalReward = 30.0f;
    public const float ConnectionReward = 0.5f;
    public const float ConnectionPenalty = -0.1f;
    public const float StabilityBonus = 0.2f;
    public const float GroupConnectivityBonus = 0.3f;
    
    // Основные свойства агента
    public string Name { get; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Speed { get; }
    public string RoadCondition { get; }
    public float CommunicationRange { get; private set; }
    public float CommunicationReliability { get; private set; }
    public static float GlobalCommunicationNoise { get; set; } = 0.05f;
    public float SignalQuality { get; private set; } = 1.0f;
    public CommunicationProtocol Protocol { get; } = new CommunicationProtocol();

    // Обновленная статистика соединений (попытки, успехи, неудачи)
    private Dictionary<string, (int attempts, int successes, int failures)> directionStats = new();
    private Dictionary<int, (int attempts, int successes, int failures)> distanceStats = new();
    private Dictionary<string, (int attempts, int successes, int failures)> conditionStats = new();

    public CarAgent(string name, float startX, float startY, float speed, 
                   float communicationRange, string roadCondition = "clear")
    {
        Name = name;
        X = startX;
        Y = startY;
        Speed = speed;
        CommunicationRange = communicationRange;
        RoadCondition = roadCondition;
        CommunicationReliability = CalculateBaseReliability();
    }

    private float CalculateBaseReliability()
    {
        return RoadCondition switch
        {
            "wet" => 0.9f,
            "icy" => 0.8f,
            _ => 0.97f
        };
    }

    // Перемещение агента в сторону цели
    public void MoveTowards(float targetX, float targetY)
    {
        float dx = targetX - X;
        float dy = targetY - Y;
        float distance = MathF.Sqrt(dx * dx + dy * dy);

        if (distance <= 0.1f) return;

        float speedMultiplier = RoadCondition switch
        {
            "wet" => 0.85f,
            "icy" => 0.75f,
            _ => 1.0f
        };

        float moveDistance = Math.Min(Speed * speedMultiplier, distance);
        X += (dx / distance) * moveDistance;
        Y += (dy / distance) * moveDistance;
        UpdateSignalQuality(distance);
    }

    // Проверка возможности связи между двумя агентами
    public bool CanCommunicate(CarAgent other)
    {        
        float distance = GetDistanceTo(other.X, other.Y);
        float maxRange = CommunicationRange * 1.8f;
        float distanceFactor = 1 - MathF.Pow(distance / maxRange, 2.5f);
        float speedFactor = 1 - Math.Clamp(Math.Abs(Speed - other.Speed) / 5f, 0, 0.3f);
        float totalQuality = CommunicationReliability * other.CommunicationReliability 
                        * distanceFactor * speedFactor 
                        * (1 - GlobalCommunicationNoise * 0.7f);
        
        if (distance < 8f) return true;
        return rand.NextDouble() < totalQuality * 1.2f;
    }

    private void UpdateSignalQuality(float movementDistance)
    {
        float stabilityFactor = 1 - Math.Clamp(movementDistance / Speed, 0, 0.2f);
        SignalQuality = Math.Clamp(SignalQuality * 0.9f + 0.1f * stabilityFactor, 0.5f, 1.0f);
    }

    public float CalculateCompositeReward(float distanceImprovement, int successfulConnections, int totalConnectionAttempts, float currentDistance)
    {
        float reward = 0f;
        float connectionRate = totalConnectionAttempts > 0 ? 
            successfulConnections / (float)totalConnectionAttempts : 1f;
        float connectionWeight = Math.Clamp(1 - currentDistance/100f, 0.3f, 0.8f);
        
        reward += MathF.Sign(distanceImprovement) * MathF.Pow(Math.Abs(distanceImprovement * 2f), 0.6f);
        reward += connectionRate * ConnectionReward * 3f * connectionWeight;
        
        if (successfulConnections >= 2) 
            reward += GroupConnectivityBonus * successfulConnections;
        
        if (currentDistance < 2f) reward += GoalReward;
        else if (currentDistance < 15f) reward += GoalReward * (1 - currentDistance/15f);
        
        return reward;
    }

    public float GetDistanceTo(float x, float y) => MathF.Sqrt((X - x) * (X - x) + (Y - y) * (Y - y));

    // Обновленный метод регистрации попытки соединения
    public void RegisterConnectionAttempt(CarAgent other, bool success)
    {
        if (other == null) return;

        // Направление
        float angle = MathF.Atan2(other.Y - Y, other.X - X);
        string direction = GetDirectionName(angle);
        UpdateStats(directionStats, direction, success);

        // Расстояние
        int distance = (int)GetDistanceTo(other.X, other.Y);
        UpdateStats(distanceStats, distance, success);

        // Дорожные условия
        string conditions = $"{RoadCondition}-{other.RoadCondition}";
        UpdateStats(conditionStats, conditions, success);
    }

    // Вспомогательный метод для обновления статистики
    private void UpdateStats<T>(Dictionary<T, (int attempts, int successes, int failures)> statsDict, T key, bool success)
    {
        var current = statsDict.GetValueOrDefault(key, (attempts: 0, successes: 0, failures: 0));
        statsDict[key] = (
            attempts: current.attempts + 1,
            successes: current.successes + (success ? 1 : 0),
            failures: current.failures + (success ? 0 : 1)
        );
    }

    private string GetDirectionName(float angle)
    {
        int sector = (int)((angle + MathF.PI) / (MathF.PI/4)) % 8;
        return sector switch {
            0 => "E", 1 => "NE", 2 => "N", 3 => "NW",
            4 => "W", 5 => "SW", 6 => "S", _ => "SE"
        };
    }

    public void AdjustCommunicationParameters()
    {
        if (directionStats.Count > 0)
        {
            var worstDirection = directionStats.MaxBy(kvp => kvp.Value.failures);
            if (worstDirection.Key == "NE" || worstDirection.Key == "NW")
                CommunicationRange *= 1.05f;
        }

        if (distanceStats.Count > 0)
        {
            var worstDistance = distanceStats.MaxBy(kvp => kvp.Value.failures);
            if (worstDistance.Key > CommunicationRange * 0.8f)
                CommunicationReliability = Math.Min(0.99f, CommunicationReliability * 1.03f);
        }
    }

    public void ResetConnectionStats()
    {
        directionStats.Clear();
        distanceStats.Clear();
        conditionStats.Clear();
    }

    public Dictionary<string, (int attempts, int successes, int failures)> GetDirectionStats() => new(directionStats);
    public Dictionary<int, (int attempts, int successes, int failures)> GetDistanceStats() => new(distanceStats);
    public Dictionary<string, (int attempts, int successes, int failures)> GetConditionStats() => new(conditionStats);
}

// Класс, отвечающий за реализацию Q-обучения
class QLearningEnvironment
{
    private readonly Dictionary<string, float> QTable = new();
    private readonly Random random = new();
    private float learningRate = 0.3f;
    private const float DiscountFactor = 0.95f;

    private float GetAdaptiveLearningRate(float qValueChange)
    {
        if (qValueChange > 5f) return Math.Min(0.5f, learningRate * 1.2f);
        if (qValueChange < 0.5f) return Math.Max(0.1f, learningRate * 0.9f);
        return learningRate;
    }

    public string ChooseAction(string state, List<string> actions, float explorationRate)
    {
        if (random.NextDouble() < explorationRate)
            return actions[random.Next(actions.Count)];

        var bestActions = actions
            .OrderByDescending(a => QTable.GetValueOrDefault($"{state}:{a}", 0))
            .Take(2)
            .ToList();
            
        return bestActions[random.Next(bestActions.Count)];
    }

    public void UpdateQValue(string state, string action, float reward, string nextState, List<string> actions)
    {
        string key = $"{state}:{action}";
        float maxNextQ = actions.Count > 0 ? actions.Max(a => QTable.GetValueOrDefault($"{nextState}:{a}", 0)) : 0;
        float currentQ = QTable.GetValueOrDefault(key, 0);
        
        float qValueChange = Math.Abs(reward + DiscountFactor * maxNextQ - currentQ);
        learningRate = GetAdaptiveLearningRate(qValueChange);
        
        QTable[key] = currentQ + learningRate * (reward + DiscountFactor * maxNextQ - currentQ);
    }

    public float GetAverageQValue() => QTable.Count == 0 ? 0 : QTable.Values.Average();
}

// Класс, моделирующий параметры протокола связи
class CommunicationProtocol
{
    private float effectiveRange = 80f;
    private float baseReliability = 0.9f;

    public void UpdateParameters(float currentSuccessRate, int activeConnections)
    {
        float congestionFactor = Math.Clamp(activeConnections / 5f, 0.5f, 2f);
        effectiveRange = 80f * (1 + currentSuccessRate) / congestionFactor;
        baseReliability = 0.9f * (1 + currentSuccessRate * 0.5f);
    }

    public bool ShouldAttemptConnection(CarAgent sender, CarAgent receiver)
    {
        float distance = sender.GetDistanceTo(receiver.X, receiver.Y);
        float adjustedRange = effectiveRange * (1 - CarAgent.GlobalCommunicationNoise);
        return distance < adjustedRange;
    }
}

class Program
{
    static void Main()
    {
        var env = new QLearningEnvironment();
        var random = new Random();
        string[] roadConditions = { "clear", "wet", "icy" };

        int numCars = 20;
        float targetX = 50.0f, targetY = 50.0f;
        int maxEpisodes = 5000;
        int maxStepsPerEpisode = 100;
        int numRuns = 5;

        var allEpisodeRewards = new List<List<float>>();
        var allAverageQValues = new List<List<float>>();
        var allBlockingProbabilities = new List<List<float>>();
        
        // Обновленные структуры для хранения статистики
        var allDirectionStats = new List<List<(string, int, int, int)>>();
        var allDistanceStats = new List<List<(int, int, int, int)>>();
        var allConditionStats = new List<List<(string, int, int, int)>>();

        float initialExplorationRate = 1.0f;
        float explorationDecay = 0.997f;
        float minExplorationRate = 0.05f;

        for (int run = 0; run < numRuns; run++)
        {
            var episodeRewards = new List<float>();
            var averageQValues = new List<float>();
            var blockingProbabilities = new List<float>();
            
            var runDirectionStats = new List<(string, int, int, int)>();
            var runDistanceStats = new List<(int, int, int, int)>();
            var runConditionStats = new List<(string, int, int, int)>();

            float currentExplorationRate = initialExplorationRate;
            float adaptiveNoise = 0.05f;

            for (int episode = 0; episode < maxEpisodes; episode++)
            {
                if (episode % 200 == 0 && episode > 1000)
                {
                    float successRate = 1 - (blockingProbabilities.Count > 0 ? 
                        blockingProbabilities.Average() : 0);
                    adaptiveNoise = 0.03f + Math.Min(0.15f, episode / 20000f) * 
                        (1 - Math.Clamp(env.GetAverageQValue()/100f, 0, 1));
                    CarAgent.GlobalCommunicationNoise = adaptiveNoise;

                    explorationDecay = episode < 3000 ? 0.998f : 0.999f;
                    currentExplorationRate = Math.Max(minExplorationRate, 
                        currentExplorationRate * explorationDecay);
                                
                    if (episode % 1000 == 0)
                    {
                        targetX = 30 + random.Next(40);
                        targetY = 30 + random.Next(40);
                    }
                }
                
                var cars = Enumerable.Range(0, numCars)
                    .Select(i => new CarAgent(
                        $"Car{i}", 
                        random.Next(0, 100), 
                        random.Next(0, 100), 
                        1.2f, 
                        85f,
                        roadConditions[random.Next(roadConditions.Length)]))
                    .ToList();

                if (episode % 100 == 0)
                {
                    float successRate = 1 - (blockingProbabilities.Count > 0 ? 
                        blockingProbabilities.Average() : 0);
                    foreach (var car in cars)
                    {
                        if (car.GetDirectionStats().Count > 0 || car.GetDistanceStats().Count > 0)
                        {
                            car.AdjustCommunicationParameters();
                        }
                        car.Protocol.UpdateParameters(successRate, cars.Count - 1);
                    }
                }

                int totalConnections = 0, failedConnections = 0;
                float totalReward = 0;

                for (int step = 0; step < maxStepsPerEpisode; step++)
                {
                    float stepReward = 0;
                    int stepConnections = 0, stepFailures = 0;

                    foreach (var car in cars)
                    {
                        string state = $"{car.X:F0},{car.Y:F0}";
                        var actions = GetEnhancedActions(car, targetX, targetY);
                        string action = env.ChooseAction(state, actions, currentExplorationRate);

                        (float newX, float newY) = GetNewPosition(car, action);
                        float distanceBefore = car.GetDistanceTo(targetX, targetY);
                        car.MoveTowards(newX, newY);
                        float distanceAfter = car.GetDistanceTo(targetX, targetY);

                        var nearbyCars = cars
                            .Where(c => c != car && car.GetDistanceTo(c.X, c.Y) < car.CommunicationRange)
                            .Take(5)
                            .ToList();

                        int successfulConnections = 0;
                        foreach (var other in nearbyCars)
                        {
                            bool canCommunicate = car.CanCommunicate(other);
                            car.RegisterConnectionAttempt(other, canCommunicate);

                            if (car.Protocol.ShouldAttemptConnection(car, other))
                            {
                                stepConnections++;
                                if (canCommunicate) successfulConnections++;
                                else stepFailures++;
                            }
                        }

                        float reward = car.CalculateCompositeReward(
                            distanceBefore - distanceAfter,
                            successfulConnections,
                            nearbyCars.Count,
                            distanceAfter
                        );

                        string newState = $"{car.X:F0},{car.Y:F0}";
                        env.UpdateQValue(state, action, reward, newState, actions);
                        stepReward += reward;
                    }

                    totalConnections += stepConnections;
                    failedConnections += stepFailures;
                    totalReward += stepReward;

                    if (cars.All(c => c.GetDistanceTo(targetX, targetY) < 3f))
                        break;
                }

                episodeRewards.Add(totalReward);
                averageQValues.Add(env.GetAverageQValue());
                
                if (totalConnections > 0)
                    blockingProbabilities.Add((float)failedConnections / totalConnections);

                currentExplorationRate = Math.Max(minExplorationRate, 
                    currentExplorationRate * explorationDecay);

                if (episode % 500 == 0 || episode == maxEpisodes - 1)
                {
                    foreach (var car in cars)
                    {
                        foreach (var kvp in car.GetDirectionStats())
                        {
                            runDirectionStats.Add((kvp.Key, kvp.Value.attempts, kvp.Value.successes, kvp.Value.failures));
                        }
                        foreach (var kvp in car.GetDistanceStats())
                        {
                            runDistanceStats.Add((kvp.Key, kvp.Value.attempts, kvp.Value.successes, kvp.Value.failures));
                        }
                        foreach (var kvp in car.GetConditionStats())
                        {
                            runConditionStats.Add((kvp.Key, kvp.Value.attempts, kvp.Value.successes, kvp.Value.failures));
                        }
                        car.ResetConnectionStats();
                    }
                }
            }

            allEpisodeRewards.Add(episodeRewards);
            allAverageQValues.Add(averageQValues);
            allBlockingProbabilities.Add(blockingProbabilities);
            allDirectionStats.Add(runDirectionStats);
            allDistanceStats.Add(runDistanceStats);
            allConditionStats.Add(runConditionStats);
        }

        SaveDataToCsv("iteration_rewards.csv", CalculateAverages(allEpisodeRewards));
        SaveDataToCsv("average_q_values.csv", CalculateAverages(allAverageQValues));
        SaveDataToCsv("blocking_probabilities.csv", CalculateAverages(allBlockingProbabilities));
        
        // Сохранение новой статистики
        SaveConnectionStats("direction_stats.csv", allDirectionStats);
        SaveConnectionStats("distance_stats.csv", allDistanceStats);
        SaveConnectionStats("condition_stats.csv", allConditionStats);
    }

    static List<string> GetEnhancedActions(CarAgent car, float targetX, float targetY)
    {
        var actions = new List<string> { 
            "up", "down", "left", "right",
            "up-left", "up-right", "down-left", "down-right"
        };

        float dx = targetX - car.X;
        float dy = targetY - car.Y;
        float dist = MathF.Sqrt(dx*dx + dy*dy);

        if (dist > 20f) {
            actions.Add($"fast-{(dx > 0 ? "right" : "left")}");
            actions.Add($"fast-{(dy > 0 ? "up" : "down")}");
        }
        else if (dist > 5f) {
            actions.Add($"precise-{(dx > 0 ? "right" : "left")}");
            actions.Add($"precise-{(dy > 0 ? "up" : "down")}");
        }

        return actions;
    }

    static (float, float) GetNewPosition(CarAgent car, string action)
    {
        float step = action.StartsWith("fast") ? 1.5f : 
                     action.StartsWith("precise") ? 0.5f : 1.0f;

        return action switch
        {
            "up" => (car.X, car.Y + step),
            "down" => (car.X, car.Y - step),
            "left" => (car.X - step, car.Y),
            "right" => (car.X + step, car.Y),
            "up-left" => (car.X - step*0.7f, car.Y + step*0.7f),
            "up-right" => (car.X + step*0.7f, car.Y + step*0.7f),
            "down-left" => (car.X - step*0.7f, car.Y - step*0.7f),
            "down-right" => (car.X + step*0.7f, car.Y - step*0.7f),
            _ => (car.X, car.Y)
        };
    }

    static List<float> CalculateAverages(List<List<float>> data) 
    { 
        return data
            .SelectMany(inner => inner.Select((value, index) => new { index, value }))
            .GroupBy(x => x.index)
            .Select(g => g.Average(x => x.value))
            .ToList();
    }

    static void SaveDataToCsv(string filePath, List<float> data) 
    {  
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Iteration,Value");
            for (int i = 0; i < data.Count; i++)
            {
                writer.WriteLine($"{i},{data[i]}");
            }
        }
    }

    // Новый метод для сохранения статистики соединений
    static void SaveConnectionStats<T>(string filePath, List<List<(T parameter, int attempts, int successes, int failures)>> allStats)
    {
        var groupedStats = allStats
            .SelectMany(inner => inner)
            .GroupBy(x => x.parameter)
            .OrderBy(g => g.Key.ToString())
            .Select(g => (
                parameter: g.Key,
                attempts: g.Sum(x => x.attempts),
                successes: g.Sum(x => x.successes),
                failures: g.Sum(x => x.failures),
                blockingPercent: (float)g.Sum(x => x.failures) / g.Sum(x => x.attempts) * 100
            ))
            .ToList();

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Parameter,Attempts,Successes,Failures,BlockingPercent");
            foreach (var stat in groupedStats)
            {
                writer.WriteLine($"{stat.parameter},{stat.attempts},{stat.successes},{stat.failures},{stat.blockingPercent:F2}");
            }
        }
    }
}