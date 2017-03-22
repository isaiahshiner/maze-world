# maze-world
A generic Grid system for playing with Maze Generation, Pathfinding, Cellular Automata, or nearly anything else.

__What is MazeWorld?__
Mazeworld is a framework based around a generic Grid and Entities that can exist within it.
It uses MonoGame Graphics to render everything, which means it is graphics hardware accelerated
and can draw very large and complex grids. In general, MazeWorld can do things like randomized 
Depth First Search maze generation, Breadth First Search solving, or Conway's Game of Life Automata.

MazeWorld was originally based on GridWorld, a Case Study for the College Board Advanced Placement Computer Science Exam.
GridWorld used some of the same concepts, but was designed as a learning aid, not a true generic playground.
The render system used Java GUI methods, and was terrible at running large Grids, for obvious reasons.

__Download:__
To run MazeWorld, download the latest "Release" .zip from the downloads section,
and extract the files anywhere. Open the folder and run the "MazeWorld.exe".

Step By Step:
1. Look at the file list and find "Release X.X.X.zip"
2. Click on "Release X.X.X.zip"
3. On the right side, click download.
4. It should prompt you to save the file. Make sure you remember where you save it to.
5. Find the file, Right Click it and click "Extract All" or "Extract Here"
6. Open the folder
7. Double click on "MazeWorld.exe"
8. Yay!

__Keyboard Controls:__
```
Escape:--------------- Closes the program.

Right Arrow: --------- Advances the Grid once.

Space:---------------- Advances the Grid continuesly based on speed.

Up and Down arrows:--- Changes speed. _Note: higher speeds may cause lag._

S Key:---------------- Turns on Salting, which will mark some Rocks for deletion during Scramble.

R Key:---------------- Resets the Grid, and changes to manual mode.

T Key:---------------- Scrambles the Grid

Y Key:---------------- Solves the Grid

L Key:---------------- Resets the Grid, and disables manual mode.

Number row 1:--------- Changes the Grid block size to 64 (default)

Number row 2:--------- Changes the Grid block size to 32

Number row 3:--------- 16

Number row 4:--------- 8

Number row 5:--------- 4

Number row 6:--------- 2 _Note: Can cause very high lag_

Z Key:---------------- On click, the block the mouse is on will print to the header.

X Key:---------------- On click, the block the mouse is on will become an Actor.

C Key:---------------- On click, the block the mouse is on will become a Rock.

NumPad Key 1 --------- Changes the block color to Red (default)

NumPad Key 2 --------- Orange

NumPad Key 3 --------- Yellow

NumPad Key 4 --------- Green

NumPad Key 5 --------- Blue

NumPad Key 6 --------- Purple

NumPad Key 7 --------- Black

NumPad Key 8 --------- Eraser (White)
```
