import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
import os
import matplotlib.colors as mcolors

# Проверка и загрузка данных для графиков обучения
'''if not os.path.exists('iteration_rewards.csv'):
    print("Файл iteration_rewards.csv не найден!")
else:
    rewards = pd.read_csv('iteration_rewards.csv')
    plt.figure(figsize=(12, 6))
    plt.plot(rewards['Iteration'], rewards['Value'], color='blue')
    #plt.title('Динамика награды по эпизодам')
    plt.xlabel('Эпизод', fontsize=18)
    plt.ylabel('Награда', fontsize=18)
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('rewards_curve.png')
    plt.show()'''



if not os.path.exists('average_q_values.csv'):
    print("Файл average_q_values.csv не найден!")
else:
    q_values = pd.read_csv('average_q_values.csv')
    plt.figure(figsize=(12, 6))
    plt.plot(q_values['Iteration'], q_values['Value'], color='green')
    #plt.title('Средние Q-значения по эпизодам')
    plt.xlabel('Эпизод', fontsize=18)
    plt.ylabel('Q-значение', fontsize=18)
    plt.xticks(fontsize=15)
    plt.yticks(fontsize=15)
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('q_values_curve.png')
    plt.show()

if not os.path.exists('blocking_probabilities.csv'):
    print("Файл blocking_probabilities.csv не найден!")
else:
    blocking = pd.read_csv('blocking_probabilities.csv')
    plt.figure(figsize=(12, 6))
    plt.plot(blocking['Iteration'], blocking['Value']*100, color='blue')
    #plt.title('Процент блокировок связи по эпизодам')
    plt.xlabel('Эпизод', fontsize=18)
    plt.ylabel('% блокировок', fontsize=18)
    plt.xticks(fontsize=15)
    plt.yticks(fontsize=15)
    plt.ylim(0, 25)
    plt.grid(True)
    plt.tight_layout()
    plt.savefig('blocking_probabilities.png')
    plt.show()

# Проверка и загрузка данных для статистики связи
# Для направлений (direction_stats)
# 1. График направлений (красный #ED1C23)
if not os.path.exists('direction_stats.csv'):
    print("Файл direction_stats.csv не найден!")
else:
    direction = pd.read_csv('direction_stats.csv')
    plt.figure(figsize=(12, 6))
    
    # Исправленная строка - используем mcolors вместо plt.colors
    colors = [mcolors.to_rgba("#241DF3B1", alpha=0.6 + 0.4*(i/len(direction))) for i in range(len(direction))]
    
    bars = plt.bar(direction['Parameter'], direction['BlockingPercent'], 
                  color=colors, edgecolor='white', linewidth=1)
    
    plt.xlabel('Направление', fontsize=16, labelpad=10)
    plt.ylabel('% блокировок', fontsize=16, labelpad=10)
    plt.xticks(rotation=45, fontsize=14, ha='right')
    plt.yticks(fontsize=14)
    plt.grid(axis='y', alpha=0.3)
    plt.tight_layout()
    plt.savefig('direction_failures.png', dpi=300, bbox_inches='tight')
    plt.show()

# 2. График расстояний (синий #005A9B)
if not os.path.exists('distance_stats.csv'):
    print("Файл distance_stats.csv не найден!")
else:
    distance = pd.read_csv('distance_stats.csv')
    distance_top = distance.sort_values('BlockingPercent', ascending=False).head(10)
    plt.figure(figsize=(12, 6))
    
    # Исправленная строка
    colors = [mcolors.to_rgba('#005A9B', alpha=0.5 + 0.5*(i/len(distance_top))) for i in range(len(distance_top))]
    
    plt.bar(distance_top['Parameter'].astype(str), distance_top['BlockingPercent'],
           color=colors, edgecolor='white', linewidth=1)
    
    plt.xlabel('Расстояние (м)', fontsize=16, labelpad=10)
    plt.ylabel('% блокировок', fontsize=16, labelpad=10)
    plt.xticks(fontsize=14)
    plt.yticks(fontsize=14)
    plt.grid(axis='y', alpha=0.3)
    plt.tight_layout()
    plt.savefig('distance_failures.png', dpi=300, bbox_inches='tight')
    plt.show()

# 3. График условий дороги (зеленый #01A94F)
if not os.path.exists('condition_stats.csv'):
    print("Файл condition_stats.csv не найден!")
else:
    condition = pd.read_csv('condition_stats.csv')
    plt.figure(figsize=(12, 6))
    
    # Исправленная строка
    colors = [mcolors.to_rgba('#01A94F', alpha=0.5 + 0.5*(i/len(condition))) for i in range(len(condition))]
    
    plt.bar(condition['Parameter'], condition['BlockingPercent'],
           color=colors, edgecolor='white', linewidth=1)
    
    plt.xlabel('Комбинация условий', fontsize=16, labelpad=10)
    plt.ylabel('% блокировок', fontsize=16, labelpad=10)
    plt.xticks(rotation=45, fontsize=14, ha='right')
    plt.yticks(fontsize=14)
    plt.grid(axis='y', alpha=0.3)
    plt.tight_layout()
    plt.savefig('condition_failures.png', dpi=300, bbox_inches='tight')
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