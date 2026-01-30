# Overview

A Work-In-Progress combat system prototype for a turn-based RPG, made in the Unity engine. The prototype is in a very early stage and should not be considered a playable demo. The primary exercise of this project was to design and implement a set of underlying systems which could support the easy definition of new combat abilities, and the complex resolution of such abilities, taking into account various factors of both the user and any targets. The prototype also has a networked foundation (P2P), allowing for multiple clients to connect and stay in sync while pieces move and resolve combat abilities.

# Features

- Extensible and modular combat skill system
- Support for fully animated 3D characters
- Complex skill resolution system which factors in lingering conditions and equipped weapons and armor
- Weapon upgrade system: tiered upgrades which increase damage, and magical enchantments which grant persistent benefits or trigger on-hit effects
- Efficient networking: minimal data transmitted to sync player actions
