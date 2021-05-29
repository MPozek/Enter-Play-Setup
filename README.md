**Warning:** The current state of this repository is incompatible with unity versions 2019+, there are some minor changes needed to persist data between loads and for examples to work again, i'll update it when i get the chance

# Enter Play Setup

A simple editor utility that allows for arbitrary logic execution before and after entering play mode.

## Installation

Copy the folder structure into your unity project's Assets folder. The exact location doesn't matter, we don't rely on any static paths.

## Examples

There are two examples in the Examples~ directory. The directory is ignored by unity by default. If you want unity to import the examples, rename the directory to "Examples" (without the tilde).

LogPlaymodeEnterLogic - displays a simple message before entering, after entering and after exiting play mode.

AlwaysStartInSceneWithIndexZero - replaces the currently open scenes before entering play mode with the scene at index 0 and restores the scene setup after exiting play mode. This example is useful for ensuring that your game always starts from the splash screen when you hit play, instead of from an arbitrary scene.

To enable the examples, locate the EnterPlaymodeSetup scriptable object and add the scriptable object instances of the examples to the list of initialization logic.

## How To Use

You can setup the logic to be executed from the scriptable object instance of EnterPlaymodeSetup. This scriptable object is editor-only and is a singleton (if one doesn't exist it will automatically be created in the project).

EnterPlaymodeSetup has a serialized list of AbstractPlaymodeEnterInitializationLogic assets. Any concrete implementation of the class can override 3 methods:

- OnPreEnterPlaymode(Scene activeScene, Scene[] openScenes) - this gets called right before we enter play mode, the game is not yet running
- OnPostEnterPlaymode(Scene activeScene, Scene[] openScenes) - this gets called right after we enter play mode, keep in mind that Awake has already run on any active scene objects
- OnPostExitPlaymode() - this gets called after we exit the game, it is no longer running and we're in pure edit mode

## How It Works

To recieve the callbacks for entering/exiting playmode, the setup object uses the EditorApplication.playmodeStateChanged event. That's all there is to it...

The harder part is perserving any data from the first callback (OnPreEnterPlaymode) to the second callback (OnPostEnterPlaymode). This is accomplished by creating copies of the attached scriptable objects and saving them as assets. These copies get destroyed once OnPostExitPlaymode is done executing so that no temporary data gets saved.
