

# VRTactileDraw: A Virtual Reality Tactile Pattern Designer for Complex Spatial Arrangements of Actuators

![Figure 1 from the paper](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/figure1.png)

## Summary

Creating tactile patterns on the body via a spatial arrangement of many tactile actuators offers many opportunities and presents a challenge, as the design space is enormous. This Unity project, released as open-source alongside our paper, presents a VR interface that enables designers to rapidly prototype complex tactile interfaces. It allows for painting strokes on a modeled body part and translates these strokes into continuous tactile patterns using an interpolation algorithm. The presented VR approach avoids several problems of traditional 2D editors. It realizes spatial 3D input using VR controllers with natural mapping and intuitive spatial movements. To evaluate this approach in detail, we conducted two user studies and iteratively improved the system. The study participants gave predominantly positive feedback on the presented VR interface (SUS score 79.7, AttrakDiff "desirable"). The final system is released alongside this paper as an open-source Unity project for various tactile hardware. Please see our paper TODO insert link! for details!

## Features
* Creation of TPs in VR to use in real-world prototypes using a Raspberry Pi or other hardware to drive vibration motors
* Real-time tactile feedback while drawing, ability to replay patterns
* Reflection paint mode enabling the user to draw while generating a mirrored stroke to make symmetrical TPs
* Automatic detection of actuators with a unique ID
* Automatic generation of easy to use .json files for the TPs
* Modular protocol implementation for actuators
* Utility such as loop mode, stroke time editing, etc.
* For more information, please see our Paper at: TODO include paper link

## Download and Installation

### Requirements

First, you'll need the necessary hardware. To use this project, you need a working VR headset and a computer to run the project. You require 
* [Unity version 2019.3.7.f1 or higher](https://unity.com/)
* [.NET Framework 4+](https://www.microsoft.com/en-us/download/details.aspx?id=17851) and 
* [SteamVR](https://store.steampowered.com/)
 

### Download
 
Simply [download](https://github.com/obkaul/VRTactileDraw.git) the project as a zip file and unzip it on your computer.


### Installation

The project itself does not require an additional installation. Copy the project onto your hard drive and open it in Unity. 

## Protocol

The default protocol that is already implemented in the RaspberryCommandSender file defines the packet structure as a 104-byte array:

* first 4 bytes as checksum defined as the sum of all following values

* next 4 bytes as the packet number, starting from 0 and increasing by 1 for each new packet

* 24x4 bytes - 32-bit integer values in range 0-255. These numbers are directly mapped to the intensities/amplitude of the actuators in percent, where 0 maps to 0% and 255 to 100%.

  

If you want to change this protocol, you can implement your own in the [RaspberryCommandSender.cs](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/PatternDesigner/Scripts/Controller/RaspberryCommandSender.cs)


## Instructions on model usage

###  Already imported presets
After downloading all required software and the project itself, you can open the project with Unity.
There are three presets already imported into the project:
* HapticHead head model
![FullBodyModelHH](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/HeadModelHH.png)
* A full body model
![FullBodyModel](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/FullBodyModel.png)
* MultiWave full-body model
![FullBodyModelMW](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/FullBodyModelMW.png)

To use one of the already imported presets, activate it in Unity while disabling all the others. 

### Editing the default presets
If you want to add more actuators, remove actuators or edit the position of them on the model, do so with Unity. Drag the actuators where you wish for them to be positioned and make sure they are not inside of the model. On starting the application, the actuators will snap to the model. 
**Important:** Give each actuator a unique ID in Unity! 

Setting an actuator id can be done in the Inspector:

![VibratorId](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/VibratorId.png)

You can choose the actuator IDs freely. However, the IDs have to be distinct.

### Importing your own models
If you have a model, you can [import](https://docs.unity3d.com/Manual/ImportingAssets.html) it via Unity and place it in the work area. Ensure that your model has a Mesh Renderer, Mesh Collider, and uses the Intensity Display script.

![Components](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/ModelComponents.png)
 
Each model has its own ActuatorHolder in which all the actuators are positioned:
 
![ActuatorHolder](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/ActuatorHolder.png)

Additionally, the ActuatorHolder has the VibratorMesh script attached to it.

![ActuatorHolderScript](https://github.com/obkaul/VRTactileDraw/blob/main/Assets/ExamplesAndImages/Images/ActuatorHolderScript.png)


### Adding actuators to your model
Use the Actuator.prefab to add actuators to your model's ActuatorHolder and drag them in position via Unity's scene viewer. The snapping is done automatically on start. Ensure the actuators are not inside your model and each actuator has a unique ID.

## Instructions on usage
When starting the project, you will find yourself in front of a slider. Use your VR controller to create a new TP. Use the trigger button to interact with objects. While working on a TP, it will continuously be saved to [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) /Patterns/ (e.g., %USERNAME%\AppData\LocalLow\HCI Group, LUH\VRTactileDraw\Patterns on Windows).
You can select a previously saved TP from the initial slider menu to replay or extend it.
After creating or opening a TP to modify, the selected model with the actuators will spawn, and you can use your VR controller to draw TPs on it.

## Project Links
* Issues: https://github.com/obkaul/VRTactileDraw/issues
* Paper: TODO include paper link

## Other
Due to the current project structure, you have to ensure that the PaintMode is disabled in Unity on start as the project will crash otherwise.
  

## Authors
* Oliver Beren Kaul
* Michael Rohs
* Andreas Domin
* Benjamin Simon
* Maximilian Schrapel

Human-Computer Interaction Group, Leibniz University Hannover, Hannover, Germany

## Contact
Oliver Beren Kaul - [beren@kaul.me](mailto:beren@kaul.me)
