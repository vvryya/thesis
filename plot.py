import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
import os

# Проверка и загрузка данных для графиков обучения
if not os.path.exists('iteration_rewards.csv'):
    print("Файл iteration_rewards.csv не найден!")
else:
    rewards = pd.read_csv('iteration_rewards.csv')
    plt.figure(figsize=(12, 6))
    plt.plot(rewards['Iteration'], rewards['Value'], color='blue')
    plt.title('Динамика награды по эпизодам')
    plt.xlabel('Эпизод')
    plt.ylabel('Награда')
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('rewards_curve.png')
    plt.show()

if not os.path.exists('average_q_values.csv'):
    print("Файл average_q_values.csv не найден!")
else:
    q_values = pd.read_csv('average_q_values.csv')
    plt.figure(figsize=(12, 6))
    plt.plot(q_values['Iteration'], q_values['Value'], color='green')
    plt.title('Средние Q-значения по эпизодам')
    plt.xlabel('Эпизод')
    plt.ylabel('Q-значение')
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('q_values_curve.png')
    plt.show()

if not os.path.exists('blocking_probabilities.csv'):
    print("Файл blocking_probabilities.csv не найден!")
else:
    blocking = pd.read_csv('blocking_probabilities.csv')
    plt.figure(figsize=(12, 6))
    plt.plot(blocking['Iteration'], blocking['Value']*100, color='red')
    plt.title('Процент блокировок связи по эпизодам')
    plt.xlabel('Эпизод')
    plt.ylabel('% блокировок')
    plt.ylim(0, 25)
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('blocking_probabilities.png')
    plt.show()

# Проверка и загрузка данных для статистики связи
if not os.path.exists('direction_failures.csv'):
    print("Файл direction_failures.csv не найден!")
else:
    direction = pd.read_csv('direction_failures.csv')
    plt.figure(figsize=(12, 6))
    directions = direction['Parameter']
    failures = direction['AverageFailures']
    plt.bar(directions, failures, color=['blue', 'green', 'red', 'cyan', 'magenta', 'yellow', 'black', 'orange'])
    plt.title('Среднее количество сбоев по направлениям')
    plt.xlabel('Направление')
    plt.ylabel('Сбои')
    plt.xticks(rotation=45)
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('direction_failures.png')
    plt.show()

if not os.path.exists('distance_failures.csv'):
    print("Файл distance_failures.csv не найден!")
else:
    distance = pd.read_csv('distance_failures.csv')
    distance_top = distance.sort_values('AverageFailures', ascending=False).head(10)
    plt.figure(figsize=(12, 6))
    plt.bar(distance_top['Parameter'].astype(str), distance_top['AverageFailures'], color=plt.cm.viridis(np.linspace(0, 1, 10)))
    plt.title('Топ-10 расстояний с наибольшим числом сбоев')
    plt.xlabel('Расстояние (м)')
    plt.ylabel('Сбои')
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('distance_failures.png')
    plt.show()

if not os.path.exists('condition_failures.csv'):
    print("Файл condition_failures.csv не найден!")
else:
    condition = pd.read_csv('condition_failures.csv')
    plt.figure(figsize=(12, 6))
    conditions = condition['Parameter']
    cond_failures = condition['AverageFailures']
    plt.bar(conditions, cond_failures, color=plt.cm.plasma(np.linspace(0, 1, len(conditions))))
    plt.title('Сбои связи по условиям дороги')
    plt.xlabel('Комбинация условий')
    plt.ylabel('Сбои')
    plt.xticks(rotation=45)
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('condition_failures.png')
    plt.show()

'''# 3D визуализация качества связи
try:
    from mpl_toolkits.mplot3d import Axes3D
    
    # Создаем тестовые данные (замените на загрузку из вашего файла)
    np.random.seed(42)
    x = np.random.rand(100) * 100
    y = np.random.rand(100) * 100
    z = np.random.rand(100) * 100  # Качество связи
    
    fig = plt.figure(figsize=(12, 8))
    ax = fig.add_subplot(111, projection='3d')
    
    sc = ax.scatter(x, y, z, c=z, cmap='viridis', marker='o')
    ax.set_title('3D карта качества связи')
    ax.set_xlabel('X координата')
    ax.set_ylabel('Y координата')
    ax.set_zlabel('Качество связи')
    
    fig.colorbar(sc, label='Уровень сигнала')
    plt.savefig('3d_communication_map.png')
    plt.show()

except ImportError:
    print("Для 3D графиков требуется mpl_toolkits")'''