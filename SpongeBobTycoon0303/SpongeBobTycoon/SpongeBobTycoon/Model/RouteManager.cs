using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class RouteManager
    {
        private readonly Vehicle _vehicle;
        private List<Stop> _stops = new();
        private List<RouteData> _route = new();
        private List<(int, int)> _partRouteFinal = new();
        private List<(int, int)> _routeFinal = new();
        private Queue<(int, int)> queue = new();

        public IReadOnlyList<Stop> Stops => _stops.AsReadOnly();
        public Vehicle Vehicle => _vehicle;

        public RouteManager(Vehicle v) 
        { 
            _vehicle = v;

        }

        public void AddStop(Stop stop)
        {
            _stops.Add(stop);
        }
        public bool ValidRoute()
        {
            if(_stops.Count == 0) return false;
            
            switch (_vehicle.product)
            {
               
                case Burger:
                    if(_stops.Count <4) return false;
                    bool burger = false;
                    bool meat = false;
                    bool spatula = false;
                    bool city = false;
                    foreach (Stop stop in _stops)
                    {
                        switch (stop.industry.product)
                        {
                            case Burger:
                                burger = true;
                                break;
                            case Meat:
                                meat = true;
                                break;
                            case Spatula:
                                spatula = true;
                                break;
                            case Citizen:
                                city = true;
                                break;
                            default:
                                break;
                        }
                    }
                    if(!burger ||  !meat || !spatula || !city)
                    {
                        return false;
                    }
                    break;
                case Net:
                    if (_stops.Count < 2) { return false; }
                    bool net = false;
                    city = false;
                    foreach (Stop stop in _stops)
                    {
                        if(stop.industry.product is Net)
                        {
                            net = true;
                        }
                        else if (stop.industry.product is Citizen)
                        {
                            city = true;
                        }
                    }
                    if (!net || !city)
                    {
                        return false;
                    }
                    break;
                case Petfood:
                    if (_stops.Count < 3) return false;
                    bool petfood = false;
                    bool algee = false;
                    city = false;
                    foreach (Stop stop in _stops)
                    {
                        if (stop.industry.product is Petfood)
                        {
                            petfood = true;
                        }
                        else if (stop.industry.product is Algae)
                        {
                            algee = true;
                        }
                        else if (stop.industry.product is Citizen)
                        {
                            city = true;
                        }
                    }
                    if(!petfood || !algee || !city)
                    {
                        System.Diagnostics.Debug.WriteLine("Route creation failed - not all stop requirements were met.");
                        return false;
                    }
                    break;
                case Wood:
                    if (_stops.Count < 2) { return false; }
                    bool wood = false;
                    city = false;
                    foreach (Stop stop in _stops)
                    {
                        if(stop.industry.product is Wood)
                        {
                            wood = true;
                        }
                        else if (stop.industry.product is Citizen)
                        {
                            city = true;
                        }
                    }
                    if (!wood || !city)
                    {
                        return false;
                    }
                    break;
                case Citizen:
                    if (_stops.Count < 2) { return false; }
                    bool city1 = false;
                    bool city2 = false;
                    foreach (Stop stop in _stops)
                    {
                        if (stop.industry.product is Citizen && !city1)
                        {
                            city1 = true;
                            
                        }
                        else if(stop.industry.product is Citizen || city1)
                        {
                            city2 = true;
                        }
                    }
                    if (!city1 || !city2)
                    {
                        return false;
                    }

                    break;
                default:
                    return false;
            }
            
            return true;
            //min 2 és muszáj érintenie azt a megállót amit szállít az autó és körút-e és jó irányba megy-e az útvonal
        }

        private List<(int,int)> BFS(GameModel model, Stop currentStop, Stop nextStop)
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
            int currentRouteX = currentStop.X;
            int currentRouteY = currentStop.Y;
            int d = 0;
            _partRouteFinal.Clear();
            _route.Clear();

            (int dx, int dy)[] directions =
                {
                             (-1,0),
                    (0,-1),           (0,1),
                             (1,0)
                };
            queue.Enqueue((currentRouteX, currentRouteY));
            visitedArray[currentRouteX, currentRouteY] = true;

            while ((currentRouteX != nextStop.X || currentRouteY != nextStop.Y) && queue.Count != 0)
            {
                var current = queue.Dequeue();
                currentRouteX = current.Item1;
                currentRouteY = current.Item2;
                d++;
                foreach (var (dx, dy) in directions)
                {
                    int nextX = currentRouteX + dx;
                    int nextY = currentRouteY + dy;

                    if (nextX >= 0 && nextX < model.board.GetLength(0) &&
                        nextY >= 0 && nextY < model.board.GetLength(1))
                    {
                        if ((model.board[nextX, nextY].fieldType is Road || model.board[nextX, nextY].fieldType is Bridge) && !visitedArray[nextX, nextY])
                        {
                            RouteData newRouteData = new RouteData((currentRouteX, currentRouteY), (nextX, nextY), d);
                            _route.Add(newRouteData);
                            visitedArray[nextX,nextY] = true;
                            queue.Enqueue((nextX, nextY));
                        }

                    }
                }
            }
            
            if (currentRouteX != nextStop.X || currentRouteY != nextStop.Y) //supposed to return empty list if route isn't a closed circle, doesn't really work though
            {
                return _partRouteFinal;
            }
              
            _route.Reverse();
            (int,int) start = (nextStop.X,nextStop.Y);

            int min = 5001;
            while (currentStop.X != start.Item1 || currentStop.Y != start.Item2)
            {
                foreach(RouteData r in _route)
                {
                    
                    if(start.Item1 == r.Current.x && start.Item2 == r.Current.y)
                    {
                        if(r.DistanceFromStart < min)
                        {
                            min = r.DistanceFromStart;
                        }
                    }
                }
                foreach (RouteData r in _route)
                {

                    if (start.Item1 == r.Current.x && start.Item2 == r.Current.y && r.DistanceFromStart == min)
                    {
                        _partRouteFinal.Add((r.Current.x,r.Current.y));
                        start = (r.Previous.x,r.Previous.y);
                        break;
                    }
                }
            }

            _partRouteFinal.Reverse();
            return _partRouteFinal;
        }


        public bool DesignRoute(GameModel model)
        {
            Stop current_stop;
            Stop next_stop;
            List<(int, int)> routeCurrent ;

            if (ValidRoute() == true)
            {
                for (int i = 0; i < (_stops!.Count - 1); i++)
                {
                    current_stop = _stops[i];
                    next_stop = _stops[i + 1];
                    routeCurrent = BFS(model, current_stop, next_stop);
                    if (routeCurrent.Count == 0)
                    {
                        _stops.Clear();
                        return false;
                    }

                    if (_routeFinal.Count == 0)
                    {
                        _routeFinal.AddRange(routeCurrent);
                    }
                    else{
                        //routeCurrent.RemoveAt(0);                    
                        foreach((int,int)coord in routeCurrent)
                        {
                            _routeFinal.Add(coord);
                        }
                    }
                    
                }
                
                if(_stops.Count > 1)
                {
                    routeCurrent = BFS(model, _stops[_stops.Count - 1], _stops[0]);
                    if (routeCurrent.Count == 0)
                    {
                        _stops.Clear();
                        return false;
                    }
                    //routeCurrent.RemoveAt(0);
                    foreach ((int, int) coord in routeCurrent)
                    {
                        _routeFinal.Add(coord);
                    }
                }
                _vehicle.AssignRoute(_routeFinal);
                _vehicle.x = _routeFinal.First().Item1;
                _vehicle.y = _routeFinal.First().Item2;
                return true;
            }
            else
            {
                _stops.Clear();
                return false;
            }
            

            //meghívja a legrövidebb utas metódust a kiválasztott megállókra

        }
    }
}
