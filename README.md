# V2V/V2X Vehicle Communications: Model Development and Future Possibilities

This repository contains the source code, simulation results, and evaluation data for my **bachelor’s thesis** project focused on **Vehicle-to-Vehicle (V2V)** and **Vehicle-to-Everything (V2X)** communication systems using **machine learning techniques** and **C# simulation modeling**.

---

## Project Information

- **Title:** V2V/V2X Vehicle Communications: Model Development and Future Possibilities
- **Degree:** Bachelor's thesis (Applied Informatics)
- **University:** RUDN University, Faculty of Physical, Mathematical and Natural Sciences
- **Supervisor:** Dr. Vyacheslav O. Begishev  
- **Author:** Varvara Y. Pavlova

---

## Summary 

The project aims to develop a **simulation model** of V2V/V2X communication under real-world conditions including **network noise** and **weather variability**. The model integrates **Q-learning** agents that dynamically build optimal routes based on traffic, connectivity, and safety.

---

## Technologies Used

- **C# (.NET)** – main simulation engine, agent logic, environment modeling  
- **Python** – post-processing and graph plotting  
- **Q-learning** – reinforcement learning for route optimization  

---

## Key Features

- Simulation of **V2V message exchange** and **agent routing**
- **Dynamic environment** with simulated weather (Clear, Wet, Icy)
- **Q-learning** for path optimization based on latency, signal loss, and safety
- Metrics include:
  - Packet loss by distance/weather
  - Signal quality degradation
  - Average reward over training episodes

---

## Results

- Agents successfully learn optimal routes avoiding risky areas
- Clear correlation between weather degradation and signal failure
- The model enables scalable expansion and supports future deep reinforcement learning (DQN) integration

---

## Publication
Pavlova V. Y., Begishev V. O.
Simulation model of V2V/V2X technology using machine learning methods.
Proceedings of the RUDN Conference on ICT and High-Tech System Modeling (2025), pp. 127–128.