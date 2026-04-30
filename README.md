# FlatLibrary

FlatLibrary is a custom 2D game engine and physics library built on top of the [MonoGame](https://www.monogame.net/) framework. It provides a robust foundation for building 2D games with a focus on ease of use, clean architecture, and integrated physics.

## Project Structure

The solution is divided into several core components:

- **FlatEngine**: The core engine library providing the base game loop, graphics rendering, camera management, and input handling.
- **FlatPhysics**: A 2D rigid-body physics engine supporting various shapes and collision responses.
- **FlatGame**: A sample game project demonstrating how to use the engine and physics libraries.
- **FlatTester**: A dedicated testing environment for verifying engine features and physics simulations.

## Key Features

### 🚀 FlatEngine
- **Base Game Class**: Inherit from `FlatGame` to quickly set up a game with built-in resolution handling and component management.
- **Advanced Graphics**:
  - **Camera**: Support for position, zoom, and coordinate transformations.
  - **Shapes**: High-performance primitive drawing (Circles, Polygons, Rectangles, Lines) using `BasicEffect`.
  - **Sprites**: Simplified texture rendering and sprite management.
  - **Screen**: Easy resolution and aspect ratio management.
- **Input Management**: Simplified wrappers for Keyboard and Mouse input.
- **Utilities**: Specialized math (`FlatMath`), random number generation (`FlatRandom`), and general-purpose tools (`FlatTools`).

### ⚖️ FlatPhysics
- **Rigid Body Dynamics**: Support for Static, Kinematic, and Dynamic bodies.
- **Collision Shapes**:
  - Circle
  - Rectangle / Square
  - Triangle / Equal Triangle
  - Arbitrary Polygons
- **Physical Properties**: Realistic simulation using Mass, Inertia, Restitution (bounciness), and Friction (Static & Dynamic).
- **World Simulation**: Configurable gravity, collision iterations, and world boundaries.

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- [MonoGame SDK](https://docs.monogame.net/articles/getting_started/1_setting_up_your_environment.html)

### Building the Project
1. Clone the repository.
2. Open `FlatLibrary.sln` in Visual Studio or your preferred IDE.
3. Restore NuGet packages.
4. Build the solution.
5. Run `FlatGame` or `FlatTester` to see the engine in action.

## Development

The engine is designed to be modular. You can use `FlatEngine` and `FlatPhysics` as independent libraries in your own MonoGame projects.

- **Core Logic**: Located in `FlatEngine/`
- **Physics Logic**: Located in `FlatPhysics/`
- **Examples**: Check `FlatGame/` and `FlatTester/` for implementation references.

---
*Created as a modular framework for 2D game development.*
