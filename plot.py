import pandas as pd
import matplotlib.pyplot as plt

# Чтение данных
iteration_rewards = pd.read_csv("iteration_rewards.csv")
average_q_values = pd.read_csv("average_q_values.csv")
episode_lengths = pd.read_csv("episode_lengths.csv")
exploration_rates = pd.read_csv("exploration_rates.csv")
blocking_probabilities = pd.read_csv("blocking_probabilities.csv")

# График средней награды за эпизод
plt.figure(figsize=(10, 5))
plt.plot(iteration_rewards["Итерация"], iteration_rewards["Значение"], label="Награда за итерацию")
plt.xlabel("Итерация")
plt.ylabel("Награда")
plt.title("Средняя награда за итерацию")
plt.legend()
plt.grid()
plt.show()

# График средних Q-значений
plt.figure(figsize=(10, 5))
plt.plot(average_q_values["Итерация"], average_q_values["Значение"], label="Среднее Q-значение")
plt.xlabel("Эпизод")
plt.ylabel("Среднее Q-значение")
plt.title("Средние Q-значения")
plt.legend()
plt.grid()
plt.show()

# График длины эпизодов
plt.figure(figsize=(10, 5))
plt.plot(episode_lengths["Итерация"], episode_lengths["Значение"], label="Среднее Q-значение")
plt.xlabel("Эпизод")
plt.ylabel("Длина эпизода")
plt.title("Длина эпизодов")
plt.legend()
plt.grid()
plt.show()

# expl
plt.figure(figsize=(10, 5))
plt.plot(exploration_rates["Итерация"], exploration_rates["Значение"], label="Среднее Q-значение")
plt.xlabel("Эпизод")
plt.ylabel("Exploration rate")
plt.title("Exploration rate")
plt.legend()
plt.grid()
plt.show()

# Вероятность блокировки
plt.figure(figsize=(10, 5))
plt.plot(blocking_probabilities["Итерация"], blocking_probabilities["Значение"], label="Среднее Q-значение")
plt.xlabel("Эпизод")
plt.ylabel("Вероятность блокировки")
plt.title("Вероятность блокировки")
plt.legend()
plt.grid()
plt.show()