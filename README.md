# qb.Animation

Components and behaviours for animation

## CONTENT

**IUpdatable**
Updatable interface implementation for animable objects 

**LinearIndexAnimation**
IUpdatable class, provides frame-based index animation with configurable delays, looping, and play modes. Supports linear and yoyo playback, and can be updated over time to advance the animation index.

**UpdatableManager**
Monobehaviour singleton for managing IUpdatable object register and update loop cycle.

## HOW TO INSTALL

Use the Unity package manager and the Install package from git url option.

- Install at first time,if you haven't already done so previously, the package <mark>[unity-package-manager-utilities](https://github.com/sandolkakos/unity-package-manager-utilities.git)</mark> from the following url: 
  [GitHub - sandolkakos/unity-package-manager-utilities: That package contains a utility that makes it possible to resolve Git Dependencies inside custom packages installed in your Unity project via UPM - Unity Package Manager.](https://github.com/sandolkakos/unity-package-manager-utilities.git)

- Next, install the package from the current package git URL. 
  
  All other dependencies of the package should be installed automatically.

## Dependencies

https://github.com/quanty-bandit/qb.Pattern.git
