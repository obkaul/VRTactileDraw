

# VR-Pattern-TactileDraw

## Summary


The idea behind this project is to simplify the process of creating pattern for actuators that are places on (parts of) the human body. Instead of writing files such as [this](https://github.com/USER/vrtactiledraw/tree/master/assets/examples/Pattern.json) by hand you can ease the work by using this project to import (part of) a human body model, place actuators on it and then draw it via VR controller. The program will handle the generation of the Pattern.json for the actuators.

## Features
* Creation of patterns in vr to use in real world prototypes using a raspberry to access vibration motors
* Reflection paint mode enabling the user to draw while generating a mirrored stroke to make symmetrical patterns
* Automatic detection of actuators with a unique ID
* Automatic generation of easy to use .json files for the patterns
* Modular protocol implementation for actuators
* Utility such as loop mode, stroke time editing etc.

## Download and Installation

### Requirements

First you need the necessary hardware of course. To use this project you need a working VR headset and a computer to run the project on. You require 
* [Unity](https://store.unity.com/)
* [.NET Framework 4+](https://www.microsoft.com/en-us/download/details.aspx?id=55170) and 
* [Steam](https://store.steampowered.com/)
 

### Download
 

Simply [download](https://github.com/USER/vrtactiledraw.git) the project as zip and unzip it on your computer.

### Installation

The project itself does not require additional installation, however if you are unfamiliar with the process of installing the required software or unfamiliar with updating Unity packages (they should usually keep themselves up-to-date) you can find guides here:

Additionally you can also change the standard protocol for your raspberry. For details refer to the "Protocol" section.

## Protocol

The default protocol that is already implemented in th RaspberryCommandSender defines the packet structure as a (optional 200 byte) 104 byte array:

* first 4 bytes as checksum defined as the sum of all following values

* next 4 bytes as the packet number, starting from 0 and increasing by 1 for each new packet

* 24x4 bytes - 32-bit integer values in range 0-255. This is directly mapped to the intesities/amplitude of the actuators in % where 0 maps to 0% and 255 to 100%

* optional: 24x4 bytes - 32-bit integer values in range 0-65535. This is directly mapped to actuator frequency in Hz. Will be ignored if 0.

  

The frequency part is optional as normal vibration actuators do not require frequency adjustments.

  

If you want to change this protocol you can implement your own in the [RaspberryCommandSender.cs](https://github.com/USER/vrtactiledraw/tree/master/assets/patterndesigner/scripts/controller/raspberrycommandsender.cs)


## Instructions on model usage

###  Already imported presets
After downloading all required software and the project itself you can open the project with Unity.
There are currently four presets alredy imported into the project:
* A full body model
![FullBodyModel](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/FullBodyModel.png)
* MultiWave full body model
![FullBodyModelMW](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/FullBodyModelMW.png)
* HapticHead full body model
![FullBodyModelHH](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/FullBodyModelHH.png)
* Arm model
![ArmModel](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/ArmModel.png)

To use one of the already imported presets activate it in Unity while disabling all the others.

### Editing the default presets
If you want to add more actuators, remove actuators or simply edit the position of them on the model, do so with Unity. Drag the actuators where you wish for them to be positioned and make sure they are not inside of the model. On start the actuators will snap to the model. 
**Important:** Give each actuator a unique ID in Unity! This can be done in the Inspector:
![VibratorId](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/VibratorId.png)
The IDs can be chosen freely, however the important part is the IDs being distinct.

### Importing your own models
If you have a model you can [import](https://docs.unity3d.com/Manual/ImportingAssets.html) it via Unity and place it in the work area. Make sure that your model has a Mesh Renderer, Mesh Collider and uses the Intensity Display script.
![Components](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/model_components.png)
 Each model has its own ActuatorHolder in which all the actuators are positioned:
![ActuatorHolder](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/ActuatorHolder.png)
Additionally the ActuatorHolder has the VibratorMesh script attached to it.
![ActuatorHolderScript](https://github.com/USER/vrtactiledraw/tree/master/assets/ExamplesAndImages/ActuatorHolderScript.png)
### Adding actuators to your own model
Use the Actuator.prefab to add actuators to your model's ActuatorHolder and drag them in position via the scene viewer in Unity. The snapping is done automatically on start. Make sure the actuators are not inside of your model and each of the actuators has a unique ID.

## Instructions on usage
When starting the project you will find yourself in front of a slider. Use your vr controller to create a new pattern. Use the trigger button to interact with objects. After creating the pattern it will be automatically saved to %user%\AppData\LocalLow\HCI Group, LUH\HapticHead\Patterns and can be selected from the slider menu for future uses.
After creating or opening the pattern to use, the selected model with the actuators will spawn and you can use your vr controller to draw patterns on it.

## Project Links
* Issues: https://github.com/USER/vrtactiledraw/issues
* Paper: 
## Other
Due to the current project structure you have to make sure that the PaintMode is disabled in Unity on start as the project will crash otherwise.
  
  

## Authors
Oliver Beren Kaul([kaul@hci.uni-hannover.de](mailto:kaul@hci.uni-hannover.de))
Michael Rohs
Andreas Domin
Benjamin Simon
Maximilian Schrapel

Human-Computer Interaction Group, Leibniz University Hannover, Hannover, Germany

## Contact

