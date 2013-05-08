Welcome to Snowscape 1.04

This package contains a suite of effects for the Unity Editor, which simulate snow building up, 
using shaders, mesh deformation, decals and particles.
You are free to use any elements of this package in your own projects, and distribute those projects
without my permissions.  You may not redistribute this package alone, in part or in whole.
I would appreciate credit in your projects!

Features Include: 
    -Snow Accumulation Shader (SM2.0).
    -Optimized Snow-Mound Deformation & Plowing.
    -Surface-Normal Accurate Footprints.
    -Central Effects' Control Script.
    -Fully Configured Demo Scene.
    -For Windows and OS X.
    -For Indie and Pro.
    -Simple Modular Design, Easily Imported.
    -Commented Code & Detailed Instructions.

Installation:
Import Snowscape, and you can immediately play around with the demo-scene by openeing "SnowScene".
It is recommended that you use the demo-scene as a basis for your own scenes.

Set up a scene from scratch:
There are 3 scripts that control the effects in Snowscape:
You can place "SnowGlobalControl" anywhere you like, I prefer to place it on my main ground object.
You must place "SnowFootprints" on your Character Controller.
You must place "SnowDeform" on the snow surfaces you want to rise and deform.

To set up the accumulation effect in Snowscape: 
Duplicate the geometry you wish to have snow on, 
and offset that geometry by a small amount on the y-axis (0.04 units is good).  
Set that geometry's material to a new one, select the "Snow" shader, populate the texture variables, 
and set the alpha value of the material's color to what ever level you would like the snow coverage to start at.  
Finally, tag the new geometry "SnowSurfaces".  When you run the scene, the SnowGlobalControl will automatically 
adjust the alpha level from there, to cause accumulation or melting. 

To set up the deformation effect in Snowscape:
Select the geometry you duplicated earlier to create the accumulation effect.  It should already be tagged "SnowSurfaces".  Place the "SnowDeform" script on this geometry.  
It is suggested that you only use SnowDeform on the ground, and only on one object.
It is also suggested that you are careful with the amount of polygons in the object you are deforming, because SnowDeform has to process all of them.  
In the Inspector panel of Unity, SnowDeform has some variables which can be assigned:
 -Rise Scale effects how quickly the snow accumulates.
 -Maximum Height sets the level that snow stops rising. Adjust this to prevent seeing under the mesh.
 -Plow Distance is the maximum distance at which snow around the plow will be plowed.
 -Plow Power effects how quickly plowing occurs.
 -Plow is the object that plows, typically the Character Controller.
 -Plow Particle is a particle system prefab that is instantiated when plowing occurs.

To set up footprints in Snowscape: 
Simply drag the SnowFootprints script onto your Character Controller.
You must tag the surfaces which are going to receive footprints "Ground".
You must add an AudioSource to your Character Controller, and add at least 1 AudioClip to "Foot Sounds".
In the Inspector panel of Unity, SnowFootprints has some variables which can be assigned:
 -Footprint R & L are the footprint mesh objects.
 -Decay Rate is the time it takes for footprints to be destroyed.
 -Min Speed is the slowest speed, of the main camera, at which footprints will be drawn.
 -Ground Distance is the distance to draw footprints at, so that jumping across the ground doesn't create footprints.
 -Offset allows you to adjust where footprints are drawn, useful to get footprints flush with surfaces.
 -Foot Sounds is where you put all the sounds which you want to randomly play whenever feet land.  YOU MUST ADD AT LEAST ONE SOUND.

Any particle emitters, like snow fall emitters, that you want to be controlled by Snowscape must be tagged "SnowParticles".
Snowscape will turn them on and off accordingly.

SnowGlobalControl, which controls the other scripts, has a couple of variables:
 -Snow Fall toggles snow effects on or off.
 -Transition Speed is the rate at which the effects increase or decrease.
 -Off Level is the level for Snow Fall to turn off automatically. Set this higher than 1 and snow never turns off.
 -Melt toggles melting, only works if Snow Fall is off as well.

Important Performance Notes-
If you are using SnowDeform and a high-polygon mesh, Snowscape may run very slowly.
To improve performance: 
  Under the Edit Menu, under "Project Settings", select "Time", and set "Fixed Timestep" to 0.03 or higher.
  
Notes about the shaders-
The 5 shaders included in Snowcape were all designed with Strumpy's Shader Editor.
The sgraph files are included for your own modifications.
There are 2 shaders which are not used in the demo scene but might be of use to you:
SnowInt and SnowIntSM3.  
Both of these shaders allow you to use a single mesh, and shader, for the snow accumulation effect, 
as opposed to needing duplicated meshes.
SnowInt can only handle the diffuse-textures and normal-maps of your snow and snow-less surfaces, and is ShaderModel 2.0 compliant.
SnowInt3 handles the diffuse and normal textures, as well as an emissive sparkle texture, and specularity/shininess, but it requires ShaderModel 3.0.

I hope that these shaders are useful to you, they were not included in the demo-scene because using duplicated
meshes, for the snow surfaces, ends up being more compatible, more efficient and works better with the plowing effect.

Please report any bugs to ashnfara@gmail.com
or comment on this thread: 
http://forum.unity3d.com/threads/96520-Snowscape-Realtime-snow-environment-pack.

