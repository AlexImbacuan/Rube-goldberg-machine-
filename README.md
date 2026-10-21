# Rube-goldberg-machine-

Unity 3D Rube Goldberg machine created by Alex Imbacuan.

## Unity setup

1. Open this repository as a Unity project.
2. Open `Assets/Scenes/RubeGoldberg.unity`.
3. The `MachineBuilder` object generates the machine in the editor, so it is visible and editable before pressing Play.
4. Select `MachineBuilder` and use the component context menu `Build Machine (Editor)` whenever you want to rebuild the machine after changing the script.
5. Press Play to run the physics simulation.

The generated objects are stored in the scene. Save the scene after making changes you want to keep.

The script builds the current four-stage prototype using primitive cubes and spheres. The remaining interactions use `OnCollisionEnter` and `Rigidbody` physics. Removed stages are no longer recreated when the machine is rebuilt.
