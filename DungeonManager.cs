using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;
using System.Text;
using static DungeonManager;

public enum TileType
{
    essential, random, empty, chest, enemy, wall
}

public class DungeonManager : MonoBehaviour
{
    
    [Serializable]
    public class PathTile
    {
        public TileType type;
        public Vector2 position;
        public List<Vector2> adjacentPathTiles;

        public PathTile(TileType t, Vector2 p, int min, int max, Dictionary<Vector2, TileType> currentTiles)
        {
            type = t;
            position = p;
            adjacentPathTiles = getAdjacentPath(min, max, currentTiles);
        }

        public List<Vector2> getAdjacentPath(int minBound, int maxBound, Dictionary<Vector2, TileType> currentTiles)
        {
            List<Vector2> pathTiles = new List<Vector2>();
            if (position.y + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x, position.y + 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y + 1));
            }
            if (position.x + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x + 1, position.y)))
            {
                pathTiles.Add(new Vector2(position.x + 1, position.y));
            }
            if (position.y - 1 > minBound && !currentTiles.ContainsKey(new Vector2(position.x, position.y - 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y - 1));
            }
            if (position.x - 1 >= minBound && !currentTiles.ContainsKey(new Vector2(position.x - 1, position.y)) && type != TileType.essential)
            {
                pathTiles.Add(new Vector2(position.x - 1, position.y));
            }
            return pathTiles;
        }
    }

    public Dictionary<Vector2, TileType> gridPositions = new Dictionary<Vector2, TileType>();

    public int minBound = 0, maxBound;
    

    public static Vector2 startPos;

    public Vector2 endPos;

    private List<Rect> rooms = new List<Rect>();
    

    int direction;

    private readonly Vector2[] directions = new Vector2[]
{
    new Vector2(0, 1),  // Góra
    new Vector2(1, 0),  // Prawo
    new Vector2(0, -1), // Dół
    new Vector2(-1, 0)  // Lewo
};
    private int currentDirectionIndex; // Zakładając, że startujemy skierowani w górę


    public int caIterations; // = 100; // Liczba iteracji automatu komórkowego
    public int birthLimit; // = 2;   // Limit narodzin dla automatu komórkowego
    public int deathLimit; //  = 2;   // Limit śmierci dla automatu komórkowego
    public int chanceToStartAlive; // = 999; // Szansa na początkowe "ożywienie" komórki
    
    
    public int algorithmNumber;


    public void StartDungeon()
    {
        algorithmNumber = 1;
        gridPositions.Clear();
        maxBound = Random.Range(12, 12);
        Debug.Log(algorithmNumber);
        switch (algorithmNumber)
        {
            case 1:
                BuildEssentialPath();
                BuildRandomPath();
                break;
            case 2:
                StartDungeonBSP();
                break;
            case 3:
                StartDungeonRandomWalk();
                break;
            case 4:
                StartDungeonRandomWalkWithCorridors();
                break;
            case 5:
                BuildLSystemPath();
                break;
            case 6:
                BuildEssentialPath();
                StartDungeonCellularAutomata();
                break;
            // Możesz dodać więcej przypadków dla dodatkowych algorytmów
            default:
                Debug.LogWarning("Nieznany numer algorytmu. Używam domyślnego algorytmu 1.");
                BuildEssentialPath();
                BuildRandomPath();
                break;
        }

    }

    public void StartDungeonCellularAutomata()
    {

        // Inicjalizacja pierwszej generacji
        for (int x = 0; x < maxBound; x++)
        {
            for (int y = 0; y < maxBound; y++)
            {
                int number = Random.Range(0, 100);
                //if (Random.Range(0, 100) < chanceToStartAlive)
                chanceToStartAlive = 38;
                if (number < chanceToStartAlive)
                {
                    Vector2 currentPos = new Vector2(x, y);
                    if (!gridPositions.ContainsKey(currentPos))
                    gridPositions[new Vector2(x, y)] = TileType.random;
                   //Debug.Log("chanceToStartAlive: " + chanceToStartAlive + " " + number + " " + x + " " + y );
                }
            }
        }
        //Wykonanie określonej liczby iteracji
        caIterations = 4;
        for (int i = 0; i < caIterations; i++)
        {
            DoSimulationStep();
            Debug.Log("caIterations " + i);
        }
        //SetStartAndEndPositions();
    }
    //private void SetStartAndEndPositions()
    //{
    //    // Wybór losowej pozycji startowej
    //    startPos = GetRandomPosition(TileType.empty);

    //    // Wybór losowej pozycji końcowej, różnej od startPos
    //    do
    //    {
    //        endPos = GetRandomPosition(TileType.empty);
    //    }
    //    while (endPos == startPos);
    //}

    private Vector2 GetRandomPosition(TileType tileType)
    {
        List<Vector2> possiblePositions = new List<Vector2>();
        foreach (KeyValuePair<Vector2, TileType> tile in gridPositions)
        {
            if (tile.Value == tileType)
            {
                possiblePositions.Add(tile.Key);
            }
        }

        if (possiblePositions.Count == 0)
        {
            return Vector2.zero; // lub inna domyślna wartość, jeśli nie znaleziono odpowiednich pozycji
        }

        return possiblePositions[Random.Range(0, possiblePositions.Count)];
    }

    private void DoSimulationStep()
    {
        Dictionary<Vector2, TileType> newGrid = new Dictionary<Vector2, TileType>();
        birthLimit = 3;
        deathLimit = 3;

        for (int x = 0; x < maxBound; x++)
        {
            for (int y = 0; y < maxBound; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                //Debug.Log("aliveNeighbors " + aliveNeighbors + " postion = " + x + " " + y);
                Vector2 currentPos = new Vector2(x, y);

                // Pomiń modyfikację jeśli kafelek jest częścią essential path
                if (IsEssentialPath(currentPos))
                {
                    Debug.Log("currentPos :" + currentPos);
                    continue;
                }
                
                if (gridPositions.ContainsKey(currentPos) && gridPositions[currentPos] == TileType.random)
                {
                    // Jeśli komórka jest "żywa" (empty) i ma za mało sąsiadów, staje się "martwa" (usuń z gridPositions)
                    if (aliveNeighbors <= deathLimit)
                    {
                        //Debug.Log(" deathlimit " + deathLimit);
                        newGrid.Remove(currentPos);
                    }
                    else
                    {
                         newGrid[currentPos] = TileType.random;
                    }
                }
                else if(!gridPositions.ContainsKey(currentPos) )
                {
                    // Jeśli komórka jest "martwa" (nie jest empty) i ma wystarczająco sąsiadów, "ożywa" (dodaj jako empty)
                    if (aliveNeighbors >= birthLimit)
                    {
                        newGrid[currentPos] = TileType.random;
                        //Debug.Log(" birthlimit " + birthLimit);
                    }
                }
            }
        }
        // Zaktualizuj gridPositions, zachowując kafelki essential path
        //foreach (var pos in gridPositions.Keys)
        //{
        //    if (!IsEssentialPath(pos) && newGrid.ContainsKey(pos))
        //    {
        //        gridPositions[pos] = newGrid[pos];
        //    }
        //}
        // Zaktualizuj gridPositions, zachowując kafelki essential path
        foreach (var pos in newGrid.Keys)
        {
            //if (!IsEssentialPath(pos))
            //{
                gridPositions[pos] = newGrid[pos];
            //}
        }

        // Usuń z gridPositions te, które nie są już "żywe" i nie są częścią essential path
        foreach (var pos in gridPositions.Keys.ToList()) // Używamy ToList(), aby móc modyfikować gridPositions
        {
            if (!newGrid.ContainsKey(pos) && !IsEssentialPath(pos))
            {
                gridPositions.Remove(pos);
            }
        }


    }

    private bool IsEssentialPath(Vector2 pos)
    {
        // Zakładamy, że kafelki essential path mają specjalny typ lub oznaczenie
        return gridPositions.ContainsKey(pos) && gridPositions[pos] == TileType.empty;
    }

    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighbour_x = x + i;
                int neighbour_y = y + j;

                // Jeśli jesteśmy na badanej komórce, pomiń ją
                if (i == 0 && j == 0) continue;

                // Sprawdź, czy sąsiednia komórka jest w granicach siatki
                if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= maxBound || neighbour_y >= maxBound)
                {
                    continue; // Traktuj komórki poza siatką jako "martwe"
                }
                else 
                {
                    // Sprawdź, czy sąsiednia komórka jest "żywa"
                    Vector2 neighborPos = new Vector2(neighbour_x, neighbour_y);
                    //if (gridPositions.ContainsKey(neighborPos) && gridPositions[neighborPos] == TileType.empty)
    //                if (gridPositions.ContainsKey(neighborPos) &&
    //(gridPositions[neighborPos] == TileType.random || gridPositions[neighborPos] == TileType.empty))
                        if (gridPositions.ContainsKey(neighborPos) && (gridPositions[neighborPos] == TileType.random))
                        {
                        count++;
                        }
                }
            }
        }
        return count;
    }


    public void StartDungeonRandomWalk()
    {
        gridPositions.Clear();
        //maxBound = Random.Range(88, 99);
        //Vector2 start = new Vector2(Random.Range(0, maxBound), Random.Range(0, maxBound));
        //Vector2 start = new Vector2(0, 0);
        Vector2 playerStartPos = Player.position;
        Debug.Log(" playerStartPos " + playerStartPos);


        PathTile ePath = new PathTile(TileType.essential, playerStartPos, minBound, maxBound, gridPositions);
        //PathTile ePath = new PathTile(TileType.essential, new Vector2(0, randomY), minBound, maxBound, gridPositions);


        PerformRandomWalk(playerStartPos, Random.Range(1,1));
        //PerformRandomWalkWithCorridors(playerStartPos, Random.Range(20, 20));
    }

    public void StartDungeonRandomWalkWithCorridors()
    {
        gridPositions.Clear();
        //maxBound = Random.Range(88, 99);
        //Vector2 start = new Vector2(Random.Range(0, maxBound), Random.Range(0, maxBound));
        //Vector2 start = new Vector2(0, 0);
        Vector2 playerStartPos = Player.position;
        Debug.Log(" playerStartPos " + playerStartPos);


        PathTile ePath = new PathTile(TileType.essential, playerStartPos, minBound, maxBound, gridPositions);
        //PathTile ePath = new PathTile(TileType.essential, new Vector2(0, randomY), minBound, maxBound, gridPositions);


        //PerformRandomWalk(playerStartPos, Random.Range(1,1));
        PerformRandomWalkWithCorridors(playerStartPos, Random.Range(20, 20));
    }

    //Metoda Random Walk z wieloma ścieżkami


    private void PerformRandomWalk(Vector2 start, int length)
    {
        Vector2 currentPos = start;
        gridPositions[currentPos] = TileType.empty;
        
        for (int i = 0; i < length; i++)
        {
            
            int stepLength = Random.Range(10, 10); // Losowa długość kroku  
            for (int step = 0; step < stepLength; step++)
            {
                if (!gridPositions.ContainsKey(currentPos))
                {
                    gridPositions[currentPos] = TileType.empty;
                }
                // Losowy kierunek
                direction = Random.Range(0, 4);
                currentPos = GetNextRandomWalkStep(currentPos, direction);
            }
        }
        endPos = currentPos;
        gridPositions[endPos] = TileType.empty;
    }
    public void StartDungeonRandomWalkMultiple()
    {
        gridPositions.Clear();
        maxBound = Random.Range(88, 100);
        // Ustawienie pozycji początkowej dla całego algorytmu
        startPos = GetRandomStartPosition();
        Vector2 currentEndPos = Vector2.zero;
        for (int i = 0; i < 5; i++) // Pięć ścieżek
        {
            //Vector2 start = (i == 0) ? startPos : GetRandomStartPosition();
            Vector2 start = Player.position;
            PerformRandomWalk(start, Random.Range(6, 7));
            currentEndPos = endPos; // Zaktualizowanie aktualnej pozycji końcowej
        }
        // Ustawienie pozycji końcowej dla całego algorytmu
        endPos = currentEndPos;
    }
    private void PerformRandomWalkWithCorridors(Vector2 start, int length)
    {
        Vector2 currentPos = start;
        gridPositions[currentPos] = TileType.empty;
        int corridorLength;

        int corridorChance;
        for (int i = 0; i < length; i++)
        {
            corridorChance = Random.Range(0, 3);
            int stepLength = Random.Range(12, 16); // Losowa długość kroku  
            for (int step = 0; step < stepLength; step++)
            {
                if (!gridPositions.ContainsKey(currentPos))
                {
                    gridPositions[currentPos] = TileType.empty;
                }
                // Losowy kierunek
                direction = Random.Range(0, 4);
                corridorLength = Random.Range(3, 5);

                if (corridorChance == 1)
                {
                    while (corridorLength > 0)
                    {

                        currentPos = GetNextRandomWalkStep(currentPos, direction);
                        corridorLength--;
                        if (!gridPositions.ContainsKey(currentPos))
                        {
                            gridPositions[currentPos] = TileType.empty;
                        }
                    }
                }

                currentPos = GetNextRandomWalkStep(currentPos, direction);
            }

            endPos = currentPos;
            gridPositions[endPos] = TileType.empty;
        }
    }

    private Vector2 GetNextRandomWalkStep(Vector2 current, int dir)
    {
        //int direction = Random.Range(0, 4);

        switch (dir)
        {
            case 0: current.x = Mathf.Clamp(current.x + 1, 0, maxBound - 1); break;
            case 1: current.x = Mathf.Clamp(current.x - 1, 0, maxBound - 1); break;
            case 2: current.y = Mathf.Clamp(current.y + 1, 0, maxBound - 1); break;
            case 3: current.y = Mathf.Clamp(current.y - 1, 0, maxBound - 1); break;
        }
        return current;
    }

    private void PerformRandomWalkWithChambers(Vector2 start, int length, int chamberCount)
    {
        Vector2 currentPos = start;
         
        for (int i = 0; i < length; i++)
        {
            int stepLength = Random.Range(2, 6); // Losowa długość kroku między 2 a 6
            for (int step = 0; step < stepLength; step++)
            {
                if (!gridPositions.ContainsKey(currentPos))
                {
                    gridPositions[currentPos] = TileType.empty;
                }

                if (i % (length / chamberCount) == 0) // Tworzenie komór w losowych miejscach
                {
                    PathTile pathTile = new PathTile(TileType.random, currentPos, minBound, maxBound, gridPositions);
                    BuildRandomChamber(pathTile);
                }

                // Losowy kierunek
                direction = Random.Range(0, 4);
                currentPos = GetNextRandomWalkStep(currentPos, direction);
            }

            endPos = currentPos;
        }
    }

    private Vector2 GetRandomStartPosition()
    {
        if (gridPositions.Count == 0)
        {
            // Domyślna wartość lub logika obsługi pustej mapy
            return new Vector2(Random.Range(0, maxBound), Random.Range(0, maxBound));
        }
        List<Vector2> possibleStarts = new List<Vector2>(gridPositions.Keys);
        return possibleStarts[Random.Range(0, possibleStarts.Count)];
    }



    ////////////////////////////////////////
    ///////////////////////////////////////
    //////////////////////////////////////
    /// <BSP>

    private void StartDungeonBSP()
    {
        // Ustawienia początkowe
        Rect initialRect = new Rect(0, 0, maxBound, maxBound);
        BSPNode rootNode = new BSPNode(initialRect);

        // Rozpoczęcie procesu podziału
        rootNode.Split(4);  // Możesz dostosować głębokość podziału

        // Zapisanie pozycji startowej
        startPos = GetRoomCenter(rootNode.room);

        // Generowanie pokoi na podstawie węzłów BSP i zapisanie pozycji końcowej
        endPos = GenerateBSPRooms(rootNode);
    }
    private Vector2 GenerateBSPRooms(BSPNode node)
    {
        if (node == null) return Vector2.zero;
        if (node.left == null && node.right == null)
        {
            // To jest liść, więc generujemy pokój
            Vector2 roomCenter = GetRoomCenter(node.room);
            PathTile pathTile = new PathTile(TileType.random, roomCenter, minBound, maxBound, gridPositions);
            BuildRandomChamber(pathTile);
            return roomCenter;
        }

        // Generowanie pokoi w lewym i prawym poddrzewie
        Vector2 leftEnd = GenerateBSPRooms(node.left);
        Vector2 rightEnd = GenerateBSPRooms(node.right);

        // Tworzenie korytarza między lewym i prawym pokojem
        if (leftEnd != Vector2.zero && rightEnd != Vector2.zero)
        {
            CreateCorridor(leftEnd, rightEnd);
        }

        return (rightEnd != Vector2.zero) ? rightEnd : leftEnd;
    }
    private void CreateCorridor(Vector2 start, Vector2 end)
    {
        // Prosta implementacja tworzenia korytarza między dwoma pokojami
        Vector2 position = start;

        while (position != end)
        {
            if (!gridPositions.ContainsKey(position))
            {
                gridPositions.Add(position, TileType.empty);
            }

            // Proste przesuwanie w kierunku końca korytarza
            if (position.x != end.x)
            {
                position.x += position.x < end.x ? 1 : -1;
            }
            else if (position.y != end.y)
            {
                position.y += position.y < end.y ? 1 : -1;
            }
        }
    }
    private Vector2 GetRoomCenter(Rect room)
    {
        return new Vector2((int)(room.x + room.width / 2), (int)(room.y + room.height / 2));
    }


    public class BSPNode
    {
        public Rect room; // Przestrzeń, którą reprezentuje ten węzeł
        public BSPNode left; // Lewe "dziecko" w drzewie BSP
        public BSPNode right; // Prawe "dziecko" w drzewie BSP

        public BSPNode(Rect newRoom)
        {
            room = newRoom;
        }

        // Metoda do podziału przestrzeni na dwie części
        public void Split(int depth)
        {
            if (depth <= 0)
            {
                return; // Osiągnięto pożądaną głębokość drzewa
            }

            // Decyzja o kierunku podziału (pionowo lub poziomo)
            bool splitH = Random.Range(0, 2) == 0;

            // Utworzenie lewego i prawego pokoju
            Rect leftRoom, rightRoom;
            if (splitH)
            {
                // Podział poziomy
                float splitPoint = Random.Range(room.yMin + 1, room.yMax);
                leftRoom = new Rect(room.xMin, room.yMin, room.width, splitPoint - room.yMin);
                rightRoom = new Rect(room.xMin, splitPoint, room.width, room.yMax - splitPoint);
            }
            else
            {
                // Podział pionowy
                float splitPoint = Random.Range(room.xMin + 1, room.xMax);
                leftRoom = new Rect(room.xMin, room.yMin, splitPoint - room.xMin, room.height);
                rightRoom = new Rect(splitPoint, room.yMin, room.xMax - splitPoint, room.height);
            }

            // Utworzenie nowych węzłów i rekurencyjny podział
            left = new BSPNode(leftRoom);
            right = new BSPNode(rightRoom);

            left.Split(depth - 1); // Rekurencyjny podział dla lewego dziecka
            right.Split(depth - 1); // Rekurencyjny podział dla prawego dziecka
        }
    }


    /// </summary>
    /////////////////////////////////////
    ////////////////////////////////////////
    //////////////////////////////////////

    /// <summary>
    /// ////////////////////////////////////// LSYSTEM
    /// </summary>

    private void BuildLSystemPath()
    {
        // Definicje L-systemu
        string axiom = "F";
        Dictionary<char, string> rules = new Dictionary<char, string>
    {
        //{ 'F', "F+F−F−F+F" }
        //{ 'F', "F-F−F+F-F" }
        { 'F', "F+F−F+F-F" }
    };
        Dictionary<char, string> lSystemRules = GenerateRandomRule(12); // Generuje 5 reguł
        int iterations = 5; // Możesz dostosować liczbę iteracji

        // Generowanie ciągu
        string pathString = GenerateLSystemString(axiom, lSystemRules, iterations);

        // Interpretacja ciągu jako ruchów
        Vector2 currentPos = Player.position; // Początkowa pozycja
        gridPositions.Add(currentPos, TileType.empty);
        // Początkowy kierunek
        currentDirectionIndex = 1;
        foreach (char c in pathString)
        {
            switch (c)
            {
                case 'F':
                    // Sprawdzamy, czy ruch do przodu nie wyjdzie poza granice
                    Vector2 newPos = currentPos + directions[currentDirectionIndex];
                    if (IsWithinBounds(newPos, maxBound))
                    {
                        currentPos = newPos;
                        MarkPositionAsPath(currentPos);
                    }
                    else
                    {
                        // Obrót o 180 stopni na krawędzi
                        TurnLeft();
                        TurnLeft();
                    }
                    break;
                case '+':
                    // Obrót w lewo
                    TurnLeft();
                    break;
                case '-':
                    // Obrót w prawo
                    TurnRight();
                    break;
            }
        }

        // Ostatnia pozycja to endPos
        endPos = currentPos;
    }

    private string GenerateLSystemString(string axiom, Dictionary<char, string> rules, int iterations)
    {
        string currentString = axiom;

        for (int i = 0; i < iterations; i++)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = sb.ToString();
        }

        return currentString;
    }

    private void MarkPositionAsPath(Vector2 pos)
    {
        if (!gridPositions.ContainsKey(pos))
            gridPositions.Add(pos, TileType.empty);
    }

    private void TurnLeft()
    {
        currentDirectionIndex = (currentDirectionIndex + 3) % 4; // Obrót w lewo
    }

    private void TurnRight()
    {
        currentDirectionIndex = (currentDirectionIndex + 1) % 4; // Obrót w prawo
    }
    private bool IsWithinBounds(Vector2 position, int maxBound)
    {
        return position.x >= 0 && position.x < maxBound && position.y >= 0 && position.y < maxBound;
    }

    private Dictionary<char, string> GenerateRandomRule(int length)
    {
        StringBuilder ruleBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            if (i % 2 == 0) // Dla parzystych indeksów, wstaw 'F'
            {
                ruleBuilder.Append('F');
            }
            else // Dla nieparzystych indeksów, wybierz losowo '+', '-'
            {
                int choice = UnityEngine.Random.Range(0, 2);
                switch (choice)
                {
                    case 0:
                        ruleBuilder.Append('+');
                        break;
                    case 1:
                        ruleBuilder.Append('-');
                        break;
                }
            }
        }

        Dictionary<char, string> rule = new Dictionary<char, string>
    {
        { 'F', ruleBuilder.ToString() }
    };

        return rule;
    }

    /// <summary>
    /// ////////////////////////////ESSENTIAL
    /// </summary>

    private void BuildEssentialPath()
    {
        int randomY = Random.Range(0, maxBound + 1);
        // Używaj aktualnej pozycji gracza jako pozycji startowej
        //Vector2 playerStartPos = Player.position;

        //PathTile ePath = new PathTile(TileType.essential, playerStartPos, minBound, maxBound, gridPositions);
        PathTile ePath = new PathTile(TileType.essential, new Vector2(0, randomY), minBound, maxBound, gridPositions);
        startPos = ePath.position;
        //SetStartPos(ePath.position);

        Debug.Log(startPos);
        int boundTracker = 0;

        while (boundTracker < maxBound)
        {
            gridPositions.Add(ePath.position, TileType.empty);

            int adjacentTileCount = ePath.adjacentPathTiles.Count;

            int randomIndex = Random.Range(0, adjacentTileCount);

            Vector2 nextEPathPos;
            if (adjacentTileCount > 0)
            {
                nextEPathPos = ePath.adjacentPathTiles[randomIndex];
            }
            else
            {
                break;
            }

            PathTile nextEPath = new PathTile(TileType.essential, nextEPathPos, minBound, maxBound, gridPositions);
            if (nextEPath.position.x > ePath.position.x || (nextEPath.position.x == maxBound - 1 && Random.Range(0, 2) == 1))
            { // Update boundtracker before EPath update
                ++boundTracker;
            }

            ePath = nextEPath;
        }
        
        if (!gridPositions.ContainsKey(ePath.position))
        {
            if (Random.Range(0, 12) == 1)
            {
                gridPositions.Add(ePath.position, TileType.chest);
            } else
            gridPositions.Add(ePath.position, TileType.empty);
        }
        

        endPos = new Vector2(ePath.position.x, ePath.position.y);
        Debug.Log("endPos " + endPos);
    }


    private void BuildRandomPath()
    {
        List<PathTile> pathQueue = new List<PathTile>();

        // Dodawanie początkowych kafelków do kolejki
        foreach (KeyValuePair<Vector2, TileType> tile in gridPositions)
        {
            Vector2 tilePos = new Vector2(tile.Key.x, tile.Key.y);
            pathQueue.Add(new PathTile(TileType.random, tilePos, minBound, maxBound, gridPositions));
        }

        for (int i = 0; i < pathQueue.Count; i++)
        {
            PathTile tile = pathQueue[i];

            int adjacentTileCount = tile.adjacentPathTiles.Count;
            if (adjacentTileCount != 0)
            {
                if (Random.Range(0, 70) == 1)
                {
                    BuildRandomChamber(tile);
                }
                else if (Random.Range(0, 33) == 1 || (tile.type == TileType.random && adjacentTileCount > 1))
                {
                    int randomIndex = Random.Range(0, adjacentTileCount);
                    Vector2 newRPathPos = tile.adjacentPathTiles[randomIndex];

                    if (!gridPositions.ContainsKey(newRPathPos))
                    {
                        if (Random.Range(0, 200) == 1)
                        {
                            gridPositions.Add(newRPathPos, TileType.enemy);
                        }
                        else if (Random.Range(0, 15) == 1)
                        {
                            gridPositions.Add(newRPathPos, TileType.chest);
                        }
                        else
                        {
                            gridPositions.Add(newRPathPos, TileType.empty);
                        }

                        PathTile newRPath = new PathTile(TileType.random, newRPathPos, minBound, maxBound, gridPositions);
                        pathQueue.Add(newRPath);
                    }
                }
            }
        }
    }

    private void BuildRandomChamber(PathTile tile)
    {

        int chamberSize = Random.Range(3, 5);

        // Sprawdzenie, czy istnieją sąsiednie kafelki
        if (tile.adjacentPathTiles.Count == 0)
        {
            Debug.Log("Brak sąsiednich kafelków do utworzenia komory.");
            return; // Zakończenie metody, jeśli nie ma sąsiednich kafelków
        }

        int randomIndex = Random.Range(0, tile.adjacentPathTiles.Count);
        Vector2 chamberOrigin = tile.adjacentPathTiles[randomIndex];

        for (int x = (int)chamberOrigin.x; x < chamberOrigin.x + chamberSize; x++)
        {
            for (int y = (int)chamberOrigin.y; y < chamberOrigin.y + chamberSize; y++)
            {
                Vector2 chamberTilePos = new Vector2(x, y);
                if (!gridPositions.ContainsKey(chamberTilePos) &&
                    chamberTilePos.x < maxBound && chamberTilePos.x > 0 &&
                    chamberTilePos.y < maxBound && chamberTilePos.y > 0)
                {
                    if (Random.Range(0, 1) == 1)
                    {
                        gridPositions.Add(chamberTilePos, TileType.chest);
                    }
                    else
                    {
                        gridPositions.Add(chamberTilePos, TileType.empty);
                    }
                }
            }
        }
    }












    //private void BuildRandomChamber(PathTile tile)
    //{
    //    int chamberSize = 3,
    //        adjacentTileCount = tile.adjacentPathTiles.Count,
    //        randomIndex = Random.Range(0, adjacentTileCount);
    //    Vector2 chamberOrigin = tile.adjacentPathTiles[randomIndex];

    //    for (int x = (int)chamberOrigin.x; x < chamberOrigin.x + chamberSize; x++)
    //    {
    //        for (int y = (int)chamberOrigin.y; y < chamberOrigin.y + chamberSize; y++)
    //        {
    //            Vector2 chamberTilePos = new Vector2(x, y);
    //            if (!gridPositions.ContainsKey(chamberTilePos) &&
    //                chamberTilePos.x < maxBound && chamberTilePos.x > 0 &&
    //                chamberTilePos.y < maxBound && chamberTilePos.y > 0)

    //                if (Random.Range(0, 20) == 1)
    //                {
    //                    gridPositions.Add(chamberTilePos, TileType.chest);
    //                }
    //                else
    //                {
    //                    gridPositions.Add(chamberTilePos, TileType.empty);
    //                }

    //        }
    //    }
    //}


    //private List<Vector2> AStar(Vector2 start, Vector2 goal)
    //{
    //    List<Node> openSet = new List<Node>();
    //    HashSet<Vector2> closedSet = new HashSet<Vector2>();
    //    Node startNode = new Node(start);
    //    startNode.Cost = 0;
    //    startNode.Heuristic = Vector2.Distance(start, goal);
    //    openSet.Add(startNode);

    //    while (openSet.Count > 0)
    //    {
    //        Node current = openSet.OrderBy(node => node.TotalCost).First();
    //        if (current.Position == goal)
    //        {
    //            return ReconstructPath(current);
    //        }

    //        openSet.Remove(current);
    //        closedSet.Add(current.Position);

    //        foreach (var neighbor in GetNeighbors(current, goal))
    //        {
    //            if (closedSet.Contains(neighbor.Position) || !IsAreaFree(new Rect(neighbor.Position.x, neighbor.Position.y, 1, 1)))
    //            {
    //                continue;
    //            }

    //            if (!openSet.Any(n => n.Position == neighbor.Position))
    //            {
    //                openSet.Add(neighbor);
    //            }
    //        }
    //    }

    //    return new List<Vector2>(); // Brak ścieżki
    //}

    //private List<Vector2> ReconstructPath(Node node)
    //{
    //    List<Vector2> path = new List<Vector2>();
    //    while (node != null)
    //    {
    //        path.Add(node.Position);
    //        node = node.Parent;
    //    }
    //    path.Reverse();
    //    return path;
    //}



    //private List<Node> GetNeighbors(Node node, Vector2 goal)
    //{
    //    List<Node> neighbors = new List<Node>();

    //    // Określ możliwe kierunki ruchu (np. w lewo, w prawo, do góry, na dół)
    //    Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    //    foreach (var dir in directions)
    //    {
    //        Vector2 newPosition = node.Position + dir;
    //        if (newPosition.x >= 0 && newPosition.x < maxBound && newPosition.y >= 0 && newPosition.y < maxBound)
    //        {
    //            Node neighbor = new Node(newPosition);
    //            neighbor.Cost = node.Cost + 1; // Załóżmy, że każdy ruch kosztuje 1
    //            neighbor.Heuristic = Vector2.Distance(newPosition, goal); // Heurystyka (np. odległość euklidesowa)
    //            neighbor.Parent = node;
    //            neighbors.Add(neighbor);
    //        }
    //    }

    //    return neighbors;
    //}


    //public void SetStartPos(Vector2 newPos)
    //{
    //    startPos = newPos;
    //}

    //private bool IsAreaFree(Rect area)
    //{
    //    for (int x = (int)area.xMin; x < area.xMax; x++)
    //    {
    //        for (int y = (int)area.yMin; y < area.yMax; y++)
    //        {
    //            Vector2 pos = new Vector2(x, y);
    //            if (gridPositions.ContainsKey(pos))
    //            {
    //                return false; // Znaleziono kolizję
    //            }
    //        }
    //    }
    //    return true; // Brak kolizji
    //}


}
