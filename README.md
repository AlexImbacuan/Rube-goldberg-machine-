# Rube-goldberg-machine-

Unity 3D Rube Goldberg machine created by Alex Imbacuan.

## Unity setup

1. Open this repository as a Unity project.
2. Create an empty GameObject in your scene named `MachineBuilder`.
3. Attach `/home/runner/work/Rube-goldberg-machine-/Rube-goldberg-machine-/Assets/Scripts/RubeGoldbergMachineBuilder.cs` to `MachineBuilder`.
4. Press Play.

The script builds a 12-step chained machine at runtime using only primitive cubes/spheres and colored materials. The interactions are triggered by `OnCollisionEnter` and `OnTriggerEnter` and exercise `Rigidbody`, `Collider`, `HingeJoint`, and `SpringJoint` behavior.
