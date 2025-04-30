import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
import os

def plot_learning_curves():
    """Построение графиков обучения с использованием только matplotlib"""
    files = {
        'rewards': 'iteration_rewards.csv',
        'q_values': 'average_q_values.csv',
        'blocking': 'blocking_probabilities.csv'
    }
    
    # Проверяем наличие файлов
    for name, file in files.items():
        if not os.path.exists(file):
            print(f"Файл {file} не найден!")
            return
    
    # Загрузка данных
    rewards = pd.read_csv('iteration_rewards.csv')
    q_values = pd.read_csv('average_q_values.csv')
    blocking = pd.read_csv('blocking_probabilities.csv')
    
    # Создаем графики
    plt.figure(figsize=(12, 10))
    
    # График наград
    plt.subplot(3, 1, 1)
    plt.plot(rewards['Iteration'], rewards['Value'], color='blue')
    plt.title('Динамика награды по эпизодам')
    plt.xlabel('Эпизод')
    plt.ylabel('Награда')
    plt.grid(True)
    
    # График Q-значений
    plt.subplot(3, 1, 2)
    plt.plot(q_values['Iteration'], q_values['Value'], color='green')
    plt.title('Средние Q-значения по эпизодам')
    plt.xlabel('Эпизод')
    plt.ylabel('Q-значение')
    plt.grid(True)
    
    # График блокировок связи
    plt.subplot(3, 1, 3)
    plt.plot(blocking['Iteration'], blocking['Value']*100, color='red')
    plt.title('Процент блокировок связи по эпизодам')
    plt.xlabel('Эпизод')
    plt.ylabel('% блокировок')
    plt.ylim(0, 100)
    plt.grid(True)
    
    plt.tight_layout()
    plt.savefig('learning_curves.png')
    plt.show()

def plot_connection_stats():
    """Анализ статистики связи с использованием только matplotlib"""
    files = {
        'direction': 'direction_failures.csv',
        'distance': 'distance_failures.csv',
        'condition': 'condition_failures.csv'
    }
    
    # Проверяем наличие файлов
    for name, file in files.items():
        if not os.path.exists(file):
            print(f"Файл {file} не найден!")
            return
    
    # Загрузка данных
    direction = pd.read_csv('direction_failures.csv')
    distance = pd.read_csv('distance_failures.csv')
    condition = pd.read_csv('condition_failures.csv')
    
    # Создаем графики
    plt.figure(figsize=(12, 15))
    
    # График по направлениям
    plt.subplot(3, 1, 1)
    directions = direction['Parameter']
    failures = direction['AverageFailures']
    plt.bar(directions, failures, color=['blue', 'green', 'red', 'cyan', 'magenta', 'yellow', 'black', 'orange'])
    plt.title('Среднее количество сбоев по направлениям')
    plt.xlabel('Направление')
    plt.ylabel('Сбои')
    plt.xticks(rotation=45)
    plt.grid(True)
    
    # График по расстояниям (топ 10)
    plt.subplot(3, 1, 2)
    distance_top = distance.sort_values('AverageFailures', ascending=False).head(10)
    plt.bar(distance_top['Parameter'].astype(str), distance_top['AverageFailures'], color=plt.cm.viridis(np.linspace(0, 1, 10)))
    plt.title('Топ-10 расстояний с наибольшим числом сбоев')
    plt.xlabel('Расстояние (м)')
    plt.ylabel('Сбои')
    plt.grid(True)
    
    # График по условиям дороги
    plt.subplot(3, 1, 3)
    conditions = condition['Parameter']
    cond_failures = condition['AverageFailures']
    plt.bar(conditions, cond_failures, color=plt.cm.plasma(np.linspace(0, 1, len(conditions))))
    plt.title('Сбои связи по условиям дороги')
    plt.xlabel('Комбинация условий')
    plt.ylabel('Сбои')
    plt.xticks(rotation=45)
    plt.grid(True)
    
    plt.tight_layout()
    plt.savefig('connection_stats.png')
    plt.show()

def plot_3d_communication_map():
    """3D визуализация качества связи с использованием только matplotlib"""
    try:
        from mpl_toolkits.mplot3d import Axes3D
    except ImportError:
        print("Для 3D графиков требуется mpl_toolkits")
        return
    
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

if __name__ == "__main__":
    print("Анализ результатов обучения...")
    plot_learning_curves()
    
    print("\nАнализ статистики связи...")
    plot_connection_stats()
    
    print("\nСоздание 3D визуализации...")
    plot_3d_communication_map()