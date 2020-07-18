# UE4 Multiplayer Testbench
I always prefer to test multiplayer specific logic outside the editor so I decided to make this quick GUI app. As some developers have noticed before (me included), testing multiplayer within the editor sometimes seems to produces inaccurate results (ie certain RPC events not replicating to certain clients even if we check "reliable").
This GUI provides quick and simple way to test your multiplayer games outside the editor.

### Todo:
Code cleanup/refactoring
clientCount doesn't decrement when a client process is closed.