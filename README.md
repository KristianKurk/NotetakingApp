# Scribe
Final project for Kristian Kurktchiev and Nami Pour Sabet Ali


CONTENTS OF THIS FILE
---------------------
   
 * Introduction
 * Requirements
 * Installation
 * Configuration
 * Software
 * FAQ
 * Maintainers


INTRODUCTION
------------
Scribe is a powerful mapping and notetaking application aimed at game masters and worldbuilders of all kind, whether it be film, literature, or video games. It makes keeping track of locations in your world and organising your notes a breeze. Scribe is 100% built on the client-side, meaning that all data is kept on the user's computers. No payment and no subscription required!


REQUIREMENTS
------------
Minimum (Tested) System Requirements:
* Windows 10
* i5-5200u
* Nvidia Geforce 840M
* 8gb of RAM

No special requirements.


INSTALLATION
------------
Download the executable by either going to the releases on this page, or visiting kristiankurk.com/Scribe/Download. It is recommeded to then put the folder wherever you wish, and to create a shortcut on your desktop.


CONFIGURATION
------------
The application can be configured in so far as color themes go. To do this, click on the settings button (gear icon) on the top navbar of the application. From then, you can change the color by using the combobox.


SOFTWARE
------------
Scribe is built with WPF C#. It uses SQLite to store user data on the client-side.


FAQ
------------
**How do I change the color of the application?**
Click on the settings button (gear icon) on the top navbar of the application. From then, you can change the color by using the combobox.


**What are campaigns?**

Campaigns, in Scribe, are instances of your data. Each campaign is a seperate collection of maps, notes, and goods.


**I want to work on my campaign from another computer. How can I do that?**

In Scribe, exporting your data is easy! Just click on the settings button (gear icon), and then click on export campaign. This will create a .db file, which you can then import on to another computer. To do this, in the campaign selector window, click on import campaign and select your aforementioned .db file. All done!


**How do maps work?**

Maps are a cornerstone of Scribe. Most controls should be intuitive to anyone who's interacted with maps. You can drag the map with your mouse, and scrolling with the mouse wheel will zoom your map in or out. Right clicking on the map will present you with the option to either create a pin, or create a map. Maps can be embedded within other maps, and you can return to a parent map by zooming out all the way. This is used, for example, to have a World Map--> Country Map --> Village Map --> House Map. 


**How do pins work?**

Clicking on a created pin on the map will show you the pin editor. From there, you're given a few options: You can delete the pin, close it, or edit its contents. Unlike the notetaking page, the pin doesn't accept anything other than cleartext without formatting. If you so wish, you can also reference a note and attach it to your pin.


**How does notetaking work?**

The notetaking page allows you to create notes and edit them as you wish. The editor should be familiar to you: It's has all the common utilities such as bold, italics, underline, color, etc. From the navbar of the editor, you can also favorite or delete a specific note. Adding a note to your favorites will allow you to pin it to your dashboard, granting easy access to information. Alternatively, you can also unfavorite a button in the same way. To create notes and categories, right click on the right side of the editor. From there, you can drag and drop the notes as you like. You can embed as many notes and categories into other categories as you wish.


**What are RNG Utilities?**

RNG Utilities are helpful features in running a tabletop roleplaying game. You're given the ability to create custom data lists and then generate a select number of those values. For example, you could create a list of NPC (Non-Player Character) names, and then get a random one in the midst of a game. The RNG Utilities allow you to also generate random dice rolls, using either the specific dice or using dice notation in a textbox. 


MAINTAINERS
------------
Kristian Kurktchiev - https://kristiankurk.com

Nami Pour Sabet Ali - portfolio to be inserted
