# UE4 Testbench
![Alt text](Source/Images/rmImg.PNG?raw=true "UE4 Testbench")
I always prefer to test game logic outside the editor, especially multiplayer specific logic so I decided to make this quick GUI app. This GUI provides a quick and simple way to test your games outside the editor. It allows you to choose from some common optional parameters before creating a Client/Server instance. Exiting the application will kill all created processes.

## Multiplayer Testing
As some developers have noticed before (me included), testing multiplayer within the editor sometimes seems to produces inaccurate results (ie certain RPC events not replicating to certain clients even if we check "reliable"). This GUI is ment to solve this by avoiding in editor testing.
Use the "Create Server Instance" button and the "Create a Client Instance" button to test multiplayer games (order doesn't matter). By default clients should join the server automatically when available unless you specify it not to.

## Singleplayer Testing
Singleplayer games only need to create a client instance to test.

### Todo:
Code cleanup/refactoring<br />
clientCount doesn't decrement when a client process is closed.
