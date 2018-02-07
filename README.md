# UBlockyGame

Two basic game demos for [ublockly](https://github.com/imagicbell/ublockly).
Watch videos:

https://youtu.be/OM8UHv4IQI0

https://youtu.be/7wunwdufTdg



## HOW TO RUN

1. clone the source **[ublockly](https://github.com/imagicbell/ublockly)**.

   ```
   git clone git@github.com:imagicbell/ublockly.git
   ```

   **Make sure the two git are in the same root folder.**

2. Open **UBlocklyGame** in Unity.

3. Hit menu **UBlockly/Build Block Prefabs** and wait to finish prefabs building.

4. Open scene:

   1. Paint game: **"AllRes/Paint/Scenes/PaintGame"**
   2. Maze game:  **"AllRes/Maze/Scenes/MazeGame"**

5. Hit play and code!

I have prepared 2 code templates for each under **/ExampleCode**. Put them in folder `Application.persistentDataPath + "XmlSave/"`. After hitting play, click the second button on the right top corner to load them. Make sure you load the corresponding `xml` for each game. 

I also provide a **MazeBuilder** for building map for maze game. Just open the scene, and follow the explicit UI to build your own map.

