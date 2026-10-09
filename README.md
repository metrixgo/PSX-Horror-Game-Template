# PSX-Horror-Game-Template
A beginner-friendly template for horror games!

This is a PSX horror game template made for Unity. Although intended for horror game uses, you can absolutely use it for normal games as well.

Features
Simple! No complex code or logic involved.
Scalable! Add more features to the game and modify the code to your needs.
Effective! Contains many helpful scripts, shaders, and other assets to help set up your game.
Can’t come up with a fourth one.

Included Templates
Trigger System. It handles a lot of triggers, which includes…
Dialogue Displays
Canvas Displays
Music/Effects
Inventory
Player Abilities
Glitch Effects
And More
Cinemachine Setup
Smooth First Person Controller (including walk, run, crouch, jump)
Camera Bobbing
Binary Movement (for doors, drawers, etc)
Main Menu & Settings
Interactables
Pickup/Putdown Item
Focusing On An Object
Translation (localization)
Keypad Template
Footstep Surface Sounds
PSX Rendering Filter
Additional Materials And Shaders

Quick Documentation
Setting Up:
Download the zip file, extract the folder and name it whatever you want. Then go to Unity Hub -> Add -> Add Project  From Disk -> Select The Folder. Click to open the project and Unity will create the game.

Trigger System:
A TriggerSequence component is all you need to perform the majority of the triggers in your game.

To use, attach a TriggerSequence to an object. Then add any triggers you want to the sequence. You can customize the settings of the sequence:

Is Physical: If checked, the triggers will fire once something enters the collider. Requires an Is Trigger collider on the object.
Self Destructs: If checked, the object is destroyed after the trigger fires, regardless if it’s physical or not.
Player Only: If checked, only objects tagged “Player” will fire the trigger via collision. Only matters if Is Physical is checked.
Types of triggers:

DisplayDialogue

Displays a dialogue. It supports rich-text tags, which means you can use tags such as <b>BOLD</b> and the code will automatically handle it.

Speaker: The speaker text displayed on top of the content.
Content: The content of the dialogue displayed with typewriter effect.
Sub: If ticked, the dialogue displays in the sub dialogue box. If not ticked, the dialogue displays in the main dialogue box.
Flash: If ticked, the player will be active while the dialogue is running. The player will not be able to skip or close the dialogue, while the dialogue closes itself. If not ticked, the dialogue pauses player movements.
Skippable (Flash off only): If ticked, the player can skip the typewriter effect. If not ticked, the dialogue cannot be closed until the typewriting effect is done.
Flash Length (Flash on only): How many seconds the dialogue stays after it finishes typing.
ChangeScreen

Change the screen from one color to another color gradually.

Start Color: The starting color.
End Color: The ending color
Length: How many seconds the screen fades from the starting color to the ending color.
Sub: If ticked, the fade displays on the sub screen layer. If not ticked, the fade displays on the main screen layer (which is above the sub screen layer).
Flash: If ticked, the player will be active while the screen is fading. If not ticked, the fade pauses player movement.
Wait For Completion (Flash off only): If ticked, the next trigger executes only when the fade is finished. If not ticked, the next trigger runs immediately.
Wait

Pauses the trigger sequence.

Length: How many seconds to wait.

Flash: If ticked, the player will be active while waiting. If not ticked, the wait pauses player movement.

DisplayPrompt

Displays a prompt on the screen.

Prompt: The text to display on the screen. Empty string means no prompts.

Color: The prompt color.

Sub: If ticked, the prompt displays on the sub prompt box. If not ticked, the fade displays on the main prompt box (which is larger and above the sub prompt).

Flash: If ticked, the prompt fades in and out. If not ticked, the prompt stays static.

ManageTasks

Modifies the task list that displays on the top-left corner.

Type: AddTask, RemoveTask, ClearTasks.

Task: If the type is AddTask, add this task to the task list. If the type is RemoveTask, remove this task from the task list. If the type is ClearTasks, this part will be hidden, and all tasks will be cleared.

ManageInventory

Modifies the player’s inventory.

Item: The item name.

Add Item: If ticked, add the item to the inventory. If not ticked, remove the item from the inventory.

PlayerCanDo

Modifies the ability for the player to do something.

Type: Look, Move, Sprint, Jump, Crouch, Interact.

Can Do: If ticked, allow the player to do the action declared in Type. If not ticked, disable the player from doing that action.

MovePlayer

Moves the player to somewhere.

Type: Location, Direction.

Vector: If the type is location, move the player to Vector (teleports the player). If the type is direction, move the player by Vector (offsets the player).

JumpscareAt

Make the player look at an object by jumpscare.

Object: The jumpscare object the player will look at.

Length: How many seconds the player will take to look at the object. Recommended length is around 0.2 seconds.

Effect: The jumpscare sound effect gets played at the start.

DisplayCanvas

Displays a canvas on the screen.

Canvas: The canvas object to display.

Effect: The sound effect gets played when the canvas is opened and closed.

Flash: If ticked, the player will be active while the canvas is displaying. The player will not be able to close the canvas, instead the canvas closes itself. If not ticked, the canvas will be closed by the player.

Flash Length (Flash on only): How many seconds the canvas displays.

PlaySound

Plays a sound.

Sound: The audio clip you want to play.

Local: If ticked, the sound will be played at a specific AudioSource. If not ticked, the sound will be played by the global effects player.

Source (Local on only): The AudioSource that plays the sound.

Is Effect (Local off only): If ticked, the sound will be played as an effect. If not ticked, the sound will be played as a music (and will be looped).

SetObject

Sets an object active or inactive.

Object: The object to set.

Set Active: If ticked, the object will be set to active. If not ticked, the object will be set to inactive.

LoadScene

Fades the screen to black and loads a scene.

Scene: The scene name to load. It must be added in Build Settings.

Length: How many seconds the screen fades to black before loading the scene.

Save: Save this scene so you can click continue on the main menu to resume the game on this scene.

DisplayEnding

Displays an ending. The sequence first displays the ending description with a typewriter effect, then displays the ending title.

Title: The title of the ending.

Description: The description of the ending.

GlitchEffect

Displays a screen glitch effect.

Type: DigitalGlitch, ScanLineJitter, VerticalJump, HorizontalShake, ColorDrift, HorizontalRipple.

Intensity: How strong the glitch type is. Set the intensity to 0 to remove the glitch.

Custom

Calls functions on objects.

Function: The function on an object you want to call.

BinaryMovement.cs:
Smoothly changes an object from one state to another state. Useful if you want to make interactions such as drawers, doors, curtains, windows, anything that has open and close states.

opened: Whether the initial state of the object is opened or closed. If it is opened, it will apply the delta movements in the positive direction. Otherwise it will apply it in the negative direction.

openLength: How many seconds it takes for the object to go from closed to opened.

closeLength: How many seconds it takes for the object to go from opened to closed.

openDelay: How many seconds after opening will the openEffect play.

closeDelay: How many seconds after closing will the closeEffect play.

deltaPosition: Change in position when opened/closed.

deltaRotation: Change in rotation when opened/closed.

deltaScale: Multiplier of scale when opened/closed.

openEffect: Effect played when opened.

closeEffect: Effect played when closed.

FocusOnObject.cs:
Allow the player to smoothly transition to another camera. Useful for reading a note on the ground, focusing on a keypad to type in password, focusing on a clue on the wall, anything you want the player to take a closer look or to focus on.

brain: The cinemachine brain the main camera is using.

focusCamera: The cinemachine camera that’s focusing on the object.

playerCamera: The cinemachine camera the player has.

transitionLength: The transition length from player camera to the focus camera.

enableMouse: While focused, unlock the cursor. This is useful if the focused object needs some interactions, such as a keypad, or a panel you need to control.

gameRenderTexture: The render texture attached to the raw image for game view.

ignoreLayer: The ignore layer. Usually you want to include your player and other triggers that you don’t want to block raycast.

focusOnEvent: The event you want to execute when the player fully focuses.

focusOffEvent: The event you want to execute when the player exits focus.

The function InteractedObject() can be called from another script on the same object, which returns the current object that’s being interacted with the mouse. Useful for making interactions such as a keypad or a panel.

FourDigitKeypad.cs:
A simple implementation of a four-digit keypad. Must be used with FocusOnObject.cs on the same object.

correctEffect: The effect gets played when you enter the correct code.

wrongEffect: The effect gets played when you enter the wrong code.

typeEffect: The effect gets played when you type in a code.

correctEvent: The event gets executed when you enter the correct code.

wrongEvent: The event gets executed when you enter the wrong code.

displays: The four display digits from left to right indexed 0, 1, 2, 3.

correctCode: The correct code for this keypad.

digitKeys: The 9 digits keys labeled from 0 through 9.

submitKey: The submit key.

deleteKey: The delete key.

Interactable.cs:
The component attached to an object to mark it as an interactable object. When player looks at this object, it will highlight itself (if there's an outline component) and display the prompt. Then the player can choose to interact with it.

prompt: The prompt gets displayed when the player looks at this object.

onInteraction: The event gets executed when the player interacts with this object.

KillerAI.cs:
A simple AI Killer that hunts the player down forever. The killer will keep moving towards the player and has supervision whatsoever.

player: The player's transform.

brain: The cinemachine brain attached to the main camera.

jumpscareCamera: The camera used to jumpscare.

reachRange: The reach of the killer.

jumpscareTime: The time it takes for the killer to grab over the player and start killing.

movingEffect: The effect gets played when the killer is moving.

jumpScareEffect: The effect gets played when the killer kills the player.

killEvent: The event you want to execute when the killer kills the player.

MainManager.cs:
The absolute necessity for every scene. Controls lots of stuff and makes triggers work.

player: The player's PlayerController component.

musicPlayer: The music player of the game.

effectsPlayer: The effects player of the game

writingEffectsPlayer: The writing effects player of the game (for dialogues).

buttonEffectsPlayer: The button clicking effect.

audioMixer: The audio mixer used for the game.

backGroundMusic: The music that gets played from the start.

writingEffect: The writing effect for dialogues.

endingEffect: The ending effect when displaying the ending.

selectEffect: The select effect when clicking on buttons.

pausedScreen: The paused screen canvas.

language: The language dropdown at the paused screen.

sensitivitySlider: The sensitivity slider at the paused screen.

masterSlider: The master slider at the paused screen.

musicSlider: The music slider at the paused screen.

effectsSlider: The effects slider at the paused screen.

dialogueScreen: The main dialogue panel.

dialogueSpeaker: The main dialogue speaker text.

dialogueContent: The main dialogue content text.

subdialogueScreen: The sub dialogue panel.

subdialogueSpeaker: The sub dialogue speaker text.

subdialogueContent: The sub dialogue content text.

subscreen: The sub screen image.

screen: The main screen image (on top of the sub screen).

superscreen: The super screen image (covering every other canvas elements).

prompt: The main prompt text.

subprompt: The sub prompt text below the main prompt.

tasksPrompt: The task list displayed on the top left.

endingScreen: The ending screen canvas.

endingTitle: The ending title text.

endingDescription: The ending description text.

endingReturnMenuButton: The return menu button after displaying ending.

startTriggers: The trigger sequence that will run when the scene loads.

PickUpItem.cs:
Picks up an item and add it to the inventory. A player can only hold one object at a time.

itemName: The name of the item that gets added to your inventory.

pickUpEffect: The effect that gets played when you pick up the item.

playerHold: The player’s hand to pick up the object. Leave it null if you want the object to be added straight to inventory without holding it with your hand.

putDownItem: The put down item associated with this pick up item. Leave it null if the item doesn’t need to be put down/does not need to be added to hand.

position: The position offset of the item relative to the player’s hand.

rotation: The rotation offset of the item relative to the player’s hand.

scale: The scale multiplier applied to the object.

oneTimeUse: Whether the additionalEffect triggers only one time while picking up, or always.

additionalEffect: The effect you want to trigger when you pick up the item.

PlayerController.cs:
Also necessity for every scene. You can control the player smoothly, including walk, run, jump, crouch. Head bobbing, interacting, and other controls are implemented.

playerCam: The player's camera.

playerHold: The player's hand.

playerBody: The player's body.

physicalLayer: The physical layer. This is where you want to put actual objects that are physical.

ignoreLayer: The ignore layer. This is where you want to put triggers you want to ignore from raycast.

surfaceSounds: The footstep sounds that will get played when player step on different surfaces. Already initialized in the sample scene.

PutDownItem.cs:
Puts down an item from hand and removes that item from the inventory.

Parameters:

putDownEffect: The effect that gets played when you put down the item.

pickUpItem: The item picked up before (that’s in the player’s hand while putting down).

oneTimeUse: Whether the additionalEffect triggers only one time while putting down, or always.

additionalEffect: The effect you want to trigger when you put down the item.

Setting Up A New Scene:
Settings...
When you are creating a new scene, you need to first set up the basic settings and configurations. For simplicity, just copy the Settings, Canvas, Player objects into your new scene, and everything will be configured correctly. Then you can customize your scene individually.

Rendering...
If you don't want the glitchy look render texture, feel free to remove it along with the game view raw image in the canvas. Remember to check the output of the main camera to none.

If you do so, the FocusOnObject.cs need to be modified, so that the InteractedObject() uses full screen mouse position directly. 

There's also a game global volume, you can also remove it/modify it to your needs. You can also create different volume for different scenes. The default volume adds a film filter to the screen as well as blackening out some of the edges of the screen.

Creating a new object...
First you need to identify which layer it belongs to. If it has no collider, you can skip this step.

Physical: All the physical stuff goes in here. All real cubes, meshes, objects, anything player can collide with (including invisible triggers!!) should go into this layer. This helps ensure player can handle crouching correctly (and possibly other interactions if you plan to implement more).

Trigger: All the invisible triggers usually goes here, such as a trigger for adding a task, a trigger that makes a jumpscare, etc. The raycast will still hit on this layer, so it's helpful to do something like block an interaction invisibly.

Ignore: This is where the triggers you don't want to block raycast goes to. The player should go into this layer because you don't want your body itself to block your raycast! Triggers that only act as triggering something but does not block interactions with other objects such as doors should also go into this layer.

Then, if the game object has an AudioSource or you plan to add one, make sure you check the audio source output to GameAudioMixer -> either Effects or Music. Usually, you want all the sounds in the scene to be effects and only the global music player to be music. Also make sure all "real sounds" have their spatial blend set to 1 so it plays in 3D world.

If this object is meant to be interacted, you need to attach an Interactable.cs and configure the settings.

If this object is meant to be stepped on by the player, you should consider tag this with the desired surface tag. There are already 12 different surface tags implemented.

Adding more layers/triggers/tags...
Remember ALWAYS add new layers/triggers/tags AFTER the existing ones. If you haven't set up anything yet, I guess it's fine, but if you already set up a lot of stuff, I suggest you to just add the new things after everything else, this way you don't scramble up the existing objects.
