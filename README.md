# 3D Boids Swarm AI System in Unity

This is a high-performance 3D Boids swarm simulation system I developed in Unity. It demonstrates advanced AI flocking behavior, high-performance optimization, and natural, steer-based movement.

---

## 🚀 Key Features

Here are the core features I implemented in this system:

* **Centralized Control:** All flocking rules (**Separation**, **Cohesion**, **Alignment**) are controlled by a central `SwarmManager` script, which allows me to tune the swarm's behavior in real-time.

* **High-Performance Neighbor Search:** I optimized the $O(n^2)$ neighbor search by implementing a **Spatial Grid** (using a hash-based dictionary). This dramatically reduced the complexity to near $O(n)$, allowing me to run hundreds of agents smoothly.

* **Intelligent Obstacle Avoidance:** I made the boids proactively avoid collisions using `Physics.SphereCast` on a dedicated "Obstacle" layer. This allows my swarm to "see" ahead and flow realistically around complex geometry, rather than just reacting to collisions.

* **Unified Target Following:** I added a "target steering" force, allowing me to direct the entire swarm towards a single, movable `Transform` in the scene.

* **Natural "Steer & Swim" Movement:** I refactored the movement logic from a simple velocity-based model to a more organic system. Boids now maintain a constant forward speed and use `Quaternion.Slerp` to smoothly **steer** towards their calculated desired direction, which perfectly mimics the natural motion of fish or birds.

## 🛠️ How It Works

* **`SwarmManager.cs`**: This script acts as the "director." It's placed on an empty GameObject and holds all the global settings (weights, speeds, layers, target). It also manages the Spatial Grid and spawns the boids.
* **`Boid.cs`**: This script is attached to the boid prefab. Each boid independently calculates its desired steering force based on the rules and then applies rotation and forward movement.

## ⚙️ Setup

1.  Attach `SwarmManager.cs` to an empty GameObject.
2.  Create your Boid prefab (e.g., a cone pointing down its positive Z-axis) and attach `Boid.cs` to it.
3.  Assign the prefab to the `Boid Prefab` slot in the `SwarmManager` inspector.
4.  Define an "Obstacles" layer in Unity and assign it to your environment geometry.
5.  Set the `Obstacle Layer` in the `SwarmManager` inspector.
6.  (Optional) Create a `Transform` to act as a target and assign it to the `Target` slot.
7.  Press Play and tune the behavior!
