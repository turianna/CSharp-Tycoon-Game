using SpongeBobTycoon.Model.FieldTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class RouteManager
    {
        public Vehicle vehicle {  get; set; }
        public List<Stop> stops { get; set; }
        public List<((int,int),(int,int),int)> route { get; set; }
        public List<(int,int)> partRouteFinal { get; set; }
        public List<(int,int)> routeFinal { get; set; }
        public Queue<(int, int)> queue { get; set; }



        public RouteManager(Vehicle v) 
        { 
            vehicle = v;
            stops = new List<Stop>();
            route = new List<((int, int), (int, int), int)>();
            routeFinal = new List<(int, int)>();
            partRouteFinal = new List<(int, int)>();
            

        }

        public bool ValidRoute()
        {
            return true;
            //min 2 és muszáj érintenie azt a megállót amit szállít az autó és körút-e és jó irányba megy-e az útvonal
        }

        private List<(int,int)> BFS(GameModel model, Stop cs, Stop ns)
        {
            queue = new Queue<(int, int)>();
            bool[,] visitedArray = new bool[model.board.GetLength(0), model.board.GetLength(1)];
            for(int i = 0; i < model.board.GetLength(0); i++)
            {
                for(int j = 0; j <  model.board.GetLength(1); j++)
                {
                    visitedArray[i, j] = false;
                }
            }
            int crx = cs.X;
            int cry = cs.Y;
            int d = 0;
            partRouteFinal.Clear();
            route.Clear();

            (int dx, int dy)[] directions =
                {
                             (-1,0),
                    (0,-1),           (0,1),
                             (1,0)
                };
            queue.Enqueue((crx, cry));
            visitedArray[crx, cry] = true;

            while ((crx != ns.X || cry != ns.Y) && queue.Count != 0)
            {
                var current = queue.Dequeue();
                crx = current.Item1;
                cry = current.Item2;
                d++;
                foreach (var (dx, dy) in directions)
                {
                    int nx = crx + dx;
                    int ny = cry + dy;

                    if (nx >= 0 && nx < model.board.GetLength(0) &&
                        ny >= 0 && ny < model.board.GetLength(1))
                    {
                        if (model.board[nx, ny].fieldType is Road && !visitedArray[nx,ny])
                        {
                            route.Add(((crx, cry), (nx, ny),d));
                            visitedArray[nx,ny] = true;
                            queue.Enqueue((nx, ny));
                        }

                    }
                }
            }
            
            if (crx != ns.X || cry != ns.Y) //supposed to return empty list if route isn't a closed circle, doesn't really work though
            {
                return partRouteFinal;
            }
              
            route.Reverse();
            (int,int) start = (ns.X,ns.Y);

            int min = 1000000;
            while (cs.X != start.Item1 || cs.Y != start.Item2)
            {
                foreach(((int x, int y) prev, (int x, int y) cur, int d)r in route)
                {
                    
                    if(start.Item1 == r.cur.x && start.Item2 == r.cur.y)
                    {
                        if(d < min)
                        {
                            min = d;
                        }
                    }
                }
                foreach (((int x, int y) prev, (int x, int y) cur, int d) r in route)
                {

                    if (start.Item1 == r.cur.x && start.Item2 == r.cur.y && d == min)
                    {
                        partRouteFinal.Add((r.cur.x,r.cur.y));
                        start = (r.prev.x,r.prev.y);

                    }
                }
            }

            partRouteFinal.Reverse();
            return partRouteFinal;
        }


        public bool DesignRoute(GameModel model)
        {
            Stop current_stop;
            Stop next_stop;
            List<(int, int)> routeCurrent ;

            if (ValidRoute() == true)
            {
                for (int i = 0; i < (stops!.Count - 1); i++)
                {
                    current_stop = stops[i];
                    next_stop = stops[i + 1];/*
                    current_stop_coordinates = (current_stop.X, current_stop.Y);
                    next_stop_coordinates = (next_stop.X, next_stop.Y);*/
                    routeCurrent = BFS(model, current_stop, next_stop);
                    if (routeCurrent.Count == 0) return false;
                    if (routeFinal.Count == 0)
                    {
                        routeFinal.AddRange(routeCurrent);
                    }
                    else{
                        //routeCurrent.RemoveAt(0);                    
                        foreach((int,int)coord in routeCurrent)
                        {
                            routeFinal.Add(coord);
                        }
                    }
                    
                }
                
                if(stops.Count > 1)
                {
                    routeCurrent = BFS(model, stops[stops.Count - 1], stops[0]);
                    if (routeCurrent.Count == 0) return false;
                    //routeCurrent.RemoveAt(0);
                    foreach ((int, int) coord in routeCurrent)
                    {
                        routeFinal.Add(coord);
                    }
                }
                
                vehicle.route = routeFinal;
                vehicle.x = routeFinal.First().Item1;
                vehicle.y = routeFinal.First().Item2;
                return true;
            }
            else
            {
                stops.Clear();
                return false;
            }
            

            //meghívja a legrövidebb utas metódust a kiválasztott megállókra

        }
    }
}
