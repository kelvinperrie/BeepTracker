This is a proof of concept phone application to help with recording kiwi signals/beeps. Tracked kiwis have a transmitter attached to their leg which sends out a signal that can be received by a handheld receiver and aerial. On 10 minute intervals the kiwi's transmitter will transmit a sequence of beeps. Those beeps can be decoded to show things like when the kiwi last left the nest, or how long ago their eggs hatched. This 'beep record' looks something like this, with eight pairs of beeps:

5,6, 2,2 2,2 11,9 7,9 10,13 5,9 4,7

These beep records are then later transcribed into an online database. Typically when you are in the field scanning for kiwi you won't have connectivity to access the online database.

The receiver and aerial are usually held by a single person while attempting to record the beeps. Some people record them on paper, some people record them on to a notes app on their phones, some people use an audio recording. All these methods are difficult when you have your hands full of a receiver and aerial.

This application is an attempt to replace a notes app on a phone with something that is easier to record the beeps. A notes app can be challenging to use as the on screen keyboard is small and holds a lot of buttons not needed for this purpose. Additionally, since the phone is being held one handed it can be hard to reach all the entry keys on the on screen keyboard.

This application has been built in .net maui and is still a work in progress. It is currently in an internal testing release (limited to specified users) on the google play store and there is an apk file under the 'installers' folder that can be used to manually install on a phone.

Features:
* Allows for the storing of beep records locally on the device i.e. doesn't require internet connectivity
* Beep records can be accessed at a later date for transcription to the online database, and then marked as 'uploaded' to track which ones have been completed
* The UI for recording beeps is skewed to one side of the phone to help with entry as it is usually used one handed (to allow for other hand to hold the aerial and receiver)
* The UI is simplified to assist with easy recording i.e no unnecessary keys like when using on screen keyboard
* The recorded date/time is automatically entered when the first beep is recorded
* The lat/lng can be retrieved from the device via a button push - recording the location the scanning was done is useful for other people and auditing

<img src="Screenshots/startpage01.png" alt="start page" width="400">

The beep records list page contains a list of all the records that have been created:

<img src="Screenshots/recordlist01annotated.png" alt="record list page" width="600">

The beep record detail page allows creation of new beep records for a bird:

<img src="Screenshots/details01annotated.png" alt="details page top" width="600">
<img src="Screenshots/details02annotated.png" alt="details page bottom" width="600">
