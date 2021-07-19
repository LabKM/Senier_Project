using System;
using System.Collections.Generic;
using UnityEngine;

using Coord = UnityEngine.Vector2Int;
public class MapGenerator : MonoBehaviour
{
   // 랜덤 변수
    [Min(1)]
    public Vector2Int wholeMapSize;
    [Range(0, 1)]
    public float RateOfRoom;
    [Range(0, 1)]
    public float ClosureRate;
    private Vector3 StartPosition;

    public GameObject TilePrefab;
    public GameObject RoomPrefab;
    public GameObject Door2Prefab;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    
    public int seed = 10;

    Coord MapCenter;

    Vector3 TileSize = new Vector3(3.556f, 0, 3.556f);

    //[HideInInspector]
    //public Transform[,] mapArray { set; get; }

    // 규칙 
    // 1. 방의 위치는 정 가운데를 기준으로 한다 
    // 2. 방은 최소 1개 (시작지점)을 간디ㅏ
    // 3. 방과 방 간의 거리는 0일 때 복도 없이 붙이고 1일 때 5개까지 벌림

    // 방의 크기는 3.556 x 3.556... 
    // 복도는  벽이 없는 방 바닥을 만들고 나중에 벽 세우기

    // 생성 방식
    // 1. 방은 3x3 정사각형 복도는 2x3 크기로 가정하고 

    private bool ClosedMap(bool[,] map, int i, int j){
        // 4면이 모두 true인지 확인
        bool result = true;
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++ y)
            {
                if ((x==0 || y == 0) && (x != 0 || y  != 0))
                {   
                    int neighbourX = i + x;
                    int neighbourY = j + y;
                    result = result & map[neighbourX, neighbourY];
                }
            }
        }
        return result;
    }

    public void SetFlagMap(Coord startPoint, bool[,] unassessableFlag){
        // 방의 형태 결정
        // wholeMapSize의 x2 + 1짜리 공간 생성
        int unassessableTile = 0;

        Coord StartRoomPoint = startPoint * 2 + Coord.one;
        Queue<Coord> way = new Queue<Coord>();

        // 전부 뚫린 형태로 일단 생성하고 
        for(int i = 0; i < unassessableFlag.GetLength(0); ++i){
            for(int j = 0; j < unassessableFlag.GetLength(1); ++j){
                if( i == 0 || j == 0 || i == unassessableFlag.GetLength(0) - 1 || j == unassessableFlag.GetLength(1) - 1
                 || ( i % 2 == 0 && j % 2 == 0 ) ){
                    // 외곽 처리 및 대각선 처리
                    unassessableTile++;
                    unassessableFlag[i,j] = true;
                }else if( i % 2 == 1 && j % 2 == 1) {
                    unassessableFlag[i,j] = false; // room
                }else{
                    unassessableFlag[i,j] = false; // way
                    way.Enqueue(new Coord(i, j)); 
                }
            }
        }

        int time = 0;
        // 랜덤으로 타일을 순서대로 뽑아서 길 막는 걸 실행
        Queue<Coord> shuffledWay = new Queue<Coord>(MapUtility.ShuffleArr<Coord>(way.ToArray(), seed));
        int numOfClosedWay = (int)(shuffledWay.Count * ClosureRate);
        for(int i = 0; i < numOfClosedWay; ++i){
            Coord randomeWayCoord = shuffledWay.Dequeue();
            unassessableFlag[randomeWayCoord.x, randomeWayCoord.y] = true;
            unassessableTile++;
            // 길이 막힌 것으로 인해 인접한 2방의 4면을 모두 막힌 건지 확인해서 그것도 막힌 것으로 처리
            // x짝수, y홀수 = 수평 이동길 // x홀수, y짝수 = 수직 이동길
            if(randomeWayCoord.x % 2 == 0) { // 수평
                if(ClosedMap(unassessableFlag, randomeWayCoord.x - 1, randomeWayCoord.y)){
                    unassessableFlag[randomeWayCoord.x - 1, randomeWayCoord.y] = true;
                    unassessableTile++;
                }
                if(ClosedMap(unassessableFlag, randomeWayCoord.x + 1, randomeWayCoord.y)){
                    unassessableFlag[randomeWayCoord.x + 1, randomeWayCoord.y] = true;
                    unassessableTile++;
                }
            }else{ // 수직
                if(ClosedMap(unassessableFlag, randomeWayCoord.x, randomeWayCoord.y - 1)){
                    unassessableFlag[randomeWayCoord.x, randomeWayCoord.y - 1] = true;
                    unassessableTile++;
                }
                if(ClosedMap(unassessableFlag, randomeWayCoord.x, randomeWayCoord.y + 1)){
                    unassessableFlag[randomeWayCoord.x, randomeWayCoord.y + 1] = true;
                    unassessableTile++;
                }
            }
            if(!MapIsFullyAccessible(unassessableFlag, unassessableTile, StartRoomPoint)){
                // 만약 모든 타일에 접근 불가능한 경우 원상복귀
                if(randomeWayCoord.x % 2 == 0) { // 수평
                    if(ClosedMap(unassessableFlag, randomeWayCoord.x - 1, randomeWayCoord.y)){
                        unassessableFlag[randomeWayCoord.x - 1, randomeWayCoord.y] = false;
                        unassessableTile--;
                    }
                    if(ClosedMap(unassessableFlag, randomeWayCoord.x + 1, randomeWayCoord.y)){
                        unassessableFlag[randomeWayCoord.x + 1, randomeWayCoord.y] = false;
                        unassessableTile--;
                    }
                }else{ // 수직
                    if(ClosedMap(unassessableFlag, randomeWayCoord.x, randomeWayCoord.y - 1)){
                        unassessableFlag[randomeWayCoord.x, randomeWayCoord.y - 1] = false;
                        unassessableTile--;
                    }
                    if(ClosedMap(unassessableFlag, randomeWayCoord.x, randomeWayCoord.y + 1)){
                        unassessableFlag[randomeWayCoord.x, randomeWayCoord.y + 1] = false;
                        unassessableTile--;
                    }
                }
                i--;
                unassessableFlag[randomeWayCoord.x, randomeWayCoord.y] = false;
                unassessableTile--;
                time++;
                shuffledWay.Enqueue(randomeWayCoord);
            }else{
                time = 0;
            }
            if(time == numOfClosedWay){
                i = numOfClosedWay;
            }
         }
    }

    Coord GetDeepestRoom(bool[,] flagMap, Coord Start){
        // 시작 지점부터 상하좌우로 한 칸씩 확인 하면서 모든 방에 최소 접근 횟수 기록해서 가장 깊이 있는 방을 리턴 
        int[,] roomMap = new int[flagMap.GetLength(0) / 2, flagMap.GetLength(1) / 2];
        for(int i = 0; i < roomMap.GetLength(0); ++i){
            for(int j = 0; j < roomMap.GetLength(1); ++j){
                roomMap[i,j] = int.MaxValue;
            }
        }

        Queue<Coord> curList = new Queue<Coord>();
        Queue<Coord> curListWait = new Queue<Coord>();
        curList.Enqueue(Start);
        roomMap[Start.x, Start.y] = 0;

        Coord DeepestRoom = Start;

        while(curList.Count > 0){
            Coord cur = curList.Dequeue();
            //Debug.Log(cur.x.ToString() + ", " + cur.y.ToString());

            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++ y)
                {
                    int neighbourX = cur.x + x;
                    int neighbourY = cur.y + y;
                    if (x != y && x==0 || y == 0)
                    {
                        if ((neighbourX >= 0 && neighbourX < roomMap.GetLength(0) && neighbourY >= 0 && neighbourY < roomMap.GetLength(1))
                            && !flagMap[cur.x * 2 + 1 + x, cur.y * 2 + 1 + y] && (roomMap[neighbourX, neighbourY] > roomMap[cur.x, cur.y] + 1)) 
                        {
                            roomMap[neighbourX, neighbourY] = roomMap[cur.x, cur.y] + 1;
                            curListWait.Enqueue(new Coord(neighbourX, neighbourY));
                            if(roomMap[DeepestRoom.x, DeepestRoom.y] < roomMap[neighbourX, neighbourY]){
                                DeepestRoom.x = neighbourX;
                                DeepestRoom.y = neighbourY;
                            }
                        }
                    }
                }
            }  

            if(curList.Count < 1){
                Queue<Coord> temp = curList;
                curList = curListWait;
                curListWait = temp;
            }
        }

        return DeepestRoom;
    }

    public void GenerateMap()
    {
        
        ShuffleCoord();

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        //생성 준비
        GameObject MapHolder = new GameObject(holderName);
        MapHolder.transform.parent = this.transform;
        StartPosition = new Vector3(-wholeMapSize.x / 2 * TileSize.x, 0, wholeMapSize.y / 2 * TileSize.z);
        MapCenter = new Coord(wholeMapSize.x / 2, wholeMapSize.y / 2);

        // 방 담을 공간 및 방의 개수  : 복도의 개수
        Room[,] roomMap = new Room[wholeMapSize.x, wholeMapSize.y];
        int NumOfRoom = (int)(RateOfRoom * wholeMapSize.x * wholeMapSize.y);
        NumOfRoom = NumOfRoom > 1 ? NumOfRoom : 2;

        // 시작 위치 결정 랜덤한 위치 하나 골라서 
        Coord StartPoint = GetRandomCoord(); 

        // 방의 형태 결정
        // wholeMapSize의 x2 + 1짜리 공간 생성
        bool[,] unassessableFlag = new bool[wholeMapSize.x * 2 + 1, wholeMapSize.y * 2 + 1];;
        SetFlagMap(StartPoint, unassessableFlag);

        GameManager gm = transform.GetComponent<GameManager>();

        // 방의 형태 기반으로 3칸씩 읽어서 구현
        // 시작 위치 결정 랜덤한 위치 하나 골라서 
        if(NumOfRoom > 0){
            NumOfRoom--;
            Coord randomCoord = StartPoint; 
            Vector3 roomPos = CoordToVector(randomCoord.x, randomCoord.y);
            roomMap[randomCoord.x, randomCoord.y] = Instantiate<GameObject>(RoomPrefab, roomPos, Quaternion.identity).GetComponent<Room>();
            Coord wayCenter = randomCoord * 2 + new Vector2Int(1, 1);
            roomMap[randomCoord.x, randomCoord.y].hallway = false;
            roomMap[randomCoord.x, randomCoord.y].startPoint = true;
            roomMap[randomCoord.x, randomCoord.y].SetDoorStyle(
                !unassessableFlag[wayCenter.x + 1, wayCenter.y], 
                !unassessableFlag[wayCenter.x - 1, wayCenter.y],
                !unassessableFlag[wayCenter.x, wayCenter.y + 1],
                !unassessableFlag[wayCenter.x, wayCenter.y - 1]);
            roomMap[randomCoord.x, randomCoord.y].transform.parent = MapHolder.transform;
            roomMap[randomCoord.x, randomCoord.y].transform.name = MapUtility.getRoomName(randomCoord.x, randomCoord.y) + "_Start";
            gm.StartPoint = roomMap[randomCoord.x, randomCoord.y].transform;
        }
        // 목표 방 위치 결정 가장 깊은 방으로 설정함
        if(NumOfRoom > 0){
            NumOfRoom--;
            Coord randomCoord = GetDeepestRoom(unassessableFlag, StartPoint); 
            Vector3 roomPos = CoordToVector(randomCoord.x, randomCoord.y);
            roomMap[randomCoord.x, randomCoord.y] = Instantiate<GameObject>(RoomPrefab, roomPos, Quaternion.identity).GetComponent<Room>();
            Coord wayCenter = randomCoord * 2 + new Vector2Int(1, 1);
            roomMap[randomCoord.x, randomCoord.y].hallway = false;
            roomMap[randomCoord.x, randomCoord.y].goalPoint = true;
            roomMap[randomCoord.x, randomCoord.y].SetDoorStyle(
                !unassessableFlag[wayCenter.x + 1, wayCenter.y], 
                !unassessableFlag[wayCenter.x - 1, wayCenter.y],
                !unassessableFlag[wayCenter.x, wayCenter.y + 1],
                !unassessableFlag[wayCenter.x, wayCenter.y - 1]);
            roomMap[randomCoord.x, randomCoord.y].transform.parent = MapHolder.transform;
            roomMap[randomCoord.x, randomCoord.y].transform.name = MapUtility.getRoomName(randomCoord.x, randomCoord.y) + "_Goal";
            gm.GoalPoint = roomMap[randomCoord.x, randomCoord.y].transform;
        }


        // 방 생성
        for(int  i = 0; i < NumOfRoom; ++i){
            Coord randomCoord = GetRandomCoord();
            if(roomMap[randomCoord.x, randomCoord.y] != null){ // roomMap이 뭔가로 채워진 경우 다시 
                i--;
            }else{ // 방 인스턴스화 하기 
                Coord wayCenter = randomCoord * 2 + new Vector2Int(1, 1);
                if( !ClosedMap(unassessableFlag, wayCenter.x, wayCenter.y) ){
                    Vector3 roomPos = CoordToVector(randomCoord.x, randomCoord.y);
                    GameObject newObstacle = Instantiate<GameObject>(RoomPrefab, roomPos, Quaternion.identity);
                    roomMap[randomCoord.x, randomCoord.y] = newObstacle.GetComponent<Room>();
                    roomMap[randomCoord.x, randomCoord.y].hallway = false;
                    roomMap[randomCoord.x, randomCoord.y].SetDoorStyle(
                        !unassessableFlag[wayCenter.x + 1, wayCenter.y], 
                        !unassessableFlag[wayCenter.x - 1, wayCenter.y],
                        !unassessableFlag[wayCenter.x, wayCenter.y + 1],
                        !unassessableFlag[wayCenter.x, wayCenter.y - 1]);
                    newObstacle.transform.parent = MapHolder.transform;
                    newObstacle.transform.name = MapUtility.getRoomName(randomCoord.x, randomCoord.y);
                }
            }
        } // Room
        // 복도 생성
        for (int i = 0; i < wholeMapSize.x; ++i)
        {
            for (int j = 0; j < wholeMapSize.y; ++j)
            {
                Coord wayPoint = new Vector2Int(i, j) * 2 + new Vector2Int(1, 1);
                if (!roomMap[i, j] && !ClosedMap(unassessableFlag, wayPoint.x, wayPoint.y))
                {
                    Vector3 roomPos = CoordToVector(i, j);
                    Room newTile = Instantiate<GameObject>(TilePrefab, roomPos, Quaternion.identity).GetComponent<Room>();
                    newTile.transform.name = MapUtility.getRoomName(i, j);
                    newTile.transform.parent = MapHolder.transform;
                    roomMap[i, j] = newTile;
                    roomMap[i, j].hallway = true;
                    roomMap[i, j].SetDoorStyle(
                        !unassessableFlag[wayPoint.x + 1, wayPoint.y], 
                        !unassessableFlag[wayPoint.x - 1, wayPoint.y],
                        !unassessableFlag[wayPoint.x, wayPoint.y + 1],
                        !unassessableFlag[wayPoint.x, wayPoint.y - 1]);
                }
            }
        } // Floor
        // 문 생성
        Door2[,] doors_h = new Door2[wholeMapSize.x - 1, wholeMapSize.y];
        Door2[,] doors_v = new Door2[wholeMapSize.x, wholeMapSize.y - 1];
        for(int i = 0; i < doors_h.GetLength(0); ++i){
            for(int j = 0; j < doors_h.GetLength(1); ++j){
                if((roomMap[i,j] != null || roomMap[i+1, j]) && !unassessableFlag[i*2+2, j*2+1]){
                    Vector3 point = (CoordToVector(i, j) + CoordToVector(i+1, j)) / 2;
                    GameObject door = Instantiate<GameObject>(Door2Prefab, point, Quaternion.Euler(0, 90, 0));
                    doors_h[i,j] = door.GetComponent<Door2>();
                    door.transform.parent = MapHolder.transform;
                }
            }
        }
        for(int i = 0; i < doors_v.GetLength(0); ++i){
            for(int j = 0; j < doors_v.GetLength(1); ++j){
                if((roomMap[i,j] != null || roomMap[i, j+1]) && !unassessableFlag[i*2+1, j*2+2]){    
                    Vector3 point = (CoordToVector(i, j) + CoordToVector(i, j+1)) / 2;
                    GameObject door = Instantiate<GameObject>(Door2Prefab, point, Quaternion.identity);
                    doors_v[i,j] = door.GetComponent<Door2>();
                    door.transform.parent = MapHolder.transform;
                }
            }
        } // Door
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount, Coord Start)
    {
        bool[,] mapFlag = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        // 시작 지점이 막혔는지 확인
        if(obstacleMap[Start.x, Start.y]){
            return false;
        }
        // 시작 지점을 기점으로 주변을 확인
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(Start);
        mapFlag[Start.x, Start.y] = true;
        int accessibleTileCount = 1;

        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++ y)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x==0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < mapFlag.GetLength(0) && neighbourY >= 0 && neighbourY < mapFlag.GetLength(1))
                        {
                            if (!mapFlag[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlag[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = obstacleMap.GetLength(0) * obstacleMap.GetLength(1) - currentObstacleCount;
        return targetAccessibleTileCount == accessibleTileCount;
    }

    void SetRoomDoorDefault(Room[,] roomMap, int i, int j){
        if(!roomMap[i, j]){
            return;
        }
        if(i == 0 ){
            if( j == 0 ){
                roomMap[i,j].SetDoorStyle(true, false, true, false);
            }else if (j == wholeMapSize.y - 1){
                roomMap[i,j].SetDoorStyle(true, false, false, true);
            }else{
                roomMap[i,j].SetDoorStyle(true, false, true, true);
            }
        }else if(i == wholeMapSize.x - 1){
            if( j == 0 ){
                roomMap[i,j].SetDoorStyle(false, true, true, false);
            }else if (j == wholeMapSize.y - 1){
                roomMap[i,j].SetDoorStyle(false, true, false, true);
            }else{
                roomMap[i,j].SetDoorStyle(false, true, true, true);
            }
        }else{
            if( j == 0 ){
                roomMap[i,j].SetDoorStyle(true, true, true, false);
            }else if (j == wholeMapSize.y - 1){
                roomMap[i,j].SetDoorStyle(true, true, false, true);
            }else{
                roomMap[i,j].SetDoorStyle(true, true, true, true); 
            }
        }
    }

    public void ShuffleCoord()
    {   
        allTileCoords = new List<Coord>();
        for (int x = 0; x < wholeMapSize.x; ++x)
        {
            for (int y = 0; y < wholeMapSize.y; ++y)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(MapUtility.ShuffleArr<Coord>(allTileCoords.ToArray(), seed));
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    Vector3 CoordToVector(int x, int y)
    {
        return StartPosition + new Vector3(TileSize.x * x, 0, -TileSize.z * y);
    }
}
