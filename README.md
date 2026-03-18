# ProNav Interceptor Simulation 

A real time, hybrid architecture simulation of an air defense interceptor missile utilizing the **Proportional Navigation (ProNav)** guidance law. 

This project demonstrates industry-standard practices by decoupling the high-performance mathematical and physical computations from the user interface and telemetry visualization.

##  Architecture Overview

To achieve deterministic physics calculations and a smooth, modern UI, this project utilizes a C-API over C++ engine approach:

1. **The Math Engine (C):** The core ProNav algorithm is written in  C and compiled as a native, unmanaged Dynamic Link Library (`ProNavEngine.dll`). This ensures maximum performance and memory control for the critical flight-path calculations.
2. **The Command & Control UI (C# / WPF):** A modern, dark-mode desktop application built with .NET and WPF. It uses the **MVVM (Model-View-ViewModel)** design pattern to cleanly separate logic from the view. The C# app uses `P/Invoke` (`DllImport`) to call the C engine 60 times per second, rendering the radar updates seamlessly.

##  The Physics: Proportional Navigation
Rather than simply chasing the target (which wastes energy and often results in a miss), the interceptor uses the ProNav algorithm to continuously calculate a collision course. 

The turning acceleration ($a_c$) is calculated based on the Line of Sight (LOS) rate:
**$a_c = N \cdot V_c \cdot \dot{\lambda}$**
* $N$ = Navigation Constant (adjustable in UI)
* $V_c$ = Closing Velocity
* $\dot{\lambda}$ = Rate of change of the Line of Sight angle

##  Key Features
* **Real Time Physics Engine:** Calculates LOS, angles, and trajectories on the fly.
* **Live Telemetry Dashboard:** Displays interceptor status, real time distance, and system alerts.
* **Interactive Parameters:** * Adjust the **Navigation Constant (N)** mid flight to see how it affects the lead angle.
  * Adjust the **Target Evasion (G)** to make the incoming threat zigzag aggressively.
* **Modern HUD Design:** A military style, dark-mode radar canvas with drop shadow effects and data binding.
