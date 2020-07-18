# UE4 Multiplayer Testbench
I always prefer to test multiplayer specific logic outside the editor so I decided to make this quick GUI app. As some developers have noticed before (me included), testing multiplayer within the editor sometimes seems to produces inaccurate results (ie certain RPC events not replicating certain clients even with "reliable" checked).
Standalone testing gives more accurate results. This GUI provides simple and fast way to test your multiplayer games outside the editor.

This GUI can also be used as a way to test mapchange/servertravel logic (and GameInstanceSpecific logic across travel) since this is impossible to do within the editor as far as I'm aware of.

### Todo:
Code cleanup/refactoring
clientCount doesn't decrement when a client process is closed.
