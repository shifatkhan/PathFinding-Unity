# Unity project demonstrating some pathfinding algorithm like A*
This was done for an assignment at Concordia University

By Shifat Khan

Project structure:
- Script > Behaviors: has all script for behaviors from the book
- Script > NPCs: Has NPC examples of using those behaviors
These behaviors were re-used from my previous [AI project](https://github.com/shifatkhan/AI_SteeringBehaviours-Unity)

- Scenes Folder:
	- Grid_Astar_Scene: to test grid/tile based pathfinding
	- POV_Astar_Scene: to test points of visibility pathfinding
	In both scenes, you can toggle between Dijstra or A* (euclidean). To do so,
	select the "TileManager" game object, and the select the appropriate "Heuristic"
	under the "Pathfinding" script component (in the inspector)
