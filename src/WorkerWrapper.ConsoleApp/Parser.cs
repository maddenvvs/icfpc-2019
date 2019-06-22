using System;
using System.Collections.Generic;
using System.Linq;
using WorkerWrapper.Domain.Geometry;
using WorkerWrapper.Domain.Models;
using WorkerWrapper.Domain.Models.Boosters;

namespace WorkerWrapper.ConsoleApp
{
    public class Parser
    {
        // input grammar
        /*x,y : Nat
        point ::= (x,y)
        map ::= repSep (point,”,”)
        BoosterCode ::= B | F | L | X
        boosterLocation ::= BoosterCode point
        obstacles ::= repSep (map,”; ”)
        boosters ::= repSep (boosterLocation,”; ”)
        task ::= map # point # obstacles # boosters*/
        
        
        private readonly string _inputText;
        private readonly bool _logEnabled;

        private const char MAIN_DELIMETER = '#';
        private const char POINT_DELIMETER = ',';
        private const string POLYGON_DELIMETER = ";";
        private const string POLYGON_INTERNAL_DELIMETER = "),";
        
        public Parser(string input, bool logEnabled = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input must be not null or empty");
            }

            _inputText = input;
            _logEnabled = logEnabled;

        }

        
        public Mine ConfigMine()
        {
            
            //example
            //(0,0),(10,0),(10,10),(0,10)#(0,0)#(4,2),(6,2),(6,7),(4,7);(5,8),(6,8),(6,9),(5,9)#B(0,1);B(1,1);F(0,2);F(1,2);L(0,3);X(0,9) - descriptoin


            var splittedParts = _inputText.Split(MAIN_DELIMETER);
            
            if (splittedParts.Length < 3)
                throw new Exception("wrong input, should contains min 3 parts");

            WriteLog(splittedParts);

            var map = ParsePolygon(splittedParts[0]);
            var workerPosition = ParsePoint(splittedParts[1]);
            var obstacles = ParsePolygonList(splittedParts[2]);

            var boosters = ParseBoosters( splittedParts.Length == 4 ? splittedParts[3] : null);
            
            if (_logEnabled)
                Console.WriteLine("parse successful");
            
            return new Mine(map,workerPosition,obstacles,boosters);
        }

        private void WriteLog(string[] splittedParts)
        {
            if (_logEnabled)
            {
                Console.WriteLine($"map = {splittedParts[0]}");
                Console.WriteLine($"initialPoint = {splittedParts[1]}");
                Console.WriteLine($"obstacles = {splittedParts[2]}");
                
                if (splittedParts.Length == 4)
                    Console.WriteLine($"boosters = {splittedParts[3]}");
            } 
        }

        private Point ParsePoint(string pointStr)
        {
            try
            {
                string truncedInput = pointStr.Substring(1, pointStr.Length - 2);
                var parts = truncedInput.Split(POINT_DELIMETER);
                if (parts.Length != 2)
                    throw new Exception($"parse point exception {pointStr}");
                
                return new Point(Convert.ToInt32(parts[0]),Convert.ToInt32(parts[1]));

            }
            catch (Exception e)
            {
                Console.WriteLine($"parse point exception = {e.Message}, input = {pointStr}");
                throw e;
            }
        }

        private Polygon ParsePolygon(string polygonStr)
        {
            try
            {
                var parts = polygonStr.Split(POLYGON_INTERNAL_DELIMETER);
                parts[parts.Length - 1] = parts[parts.Length - 1].Substring(0, parts[parts.Length - 1].Length - 1); //у последней части  убираем скобку чтобы все обрабатывались одинаково
                
                if (parts.Length < 4 )
                    throw new Exception("polygon should contain min 4 point");

                var points = parts.Select(x => ParsePoint(x + ")"));
                
                return new Polygon(points.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine($"parse polygon exception = {e.Message}, input = {polygonStr}");
                throw;
            }
        }


        private List<Polygon> ParsePolygonList(string polygonListString)
        {
            try
            {
                if (string.IsNullOrEmpty(polygonListString))
                    return new List<Polygon>();
                
                var parts = polygonListString.Split(POLYGON_DELIMETER);
                
                var polygons = parts.Select(ParsePolygon);
                
                return new List<Polygon>(polygons.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine($"parse polygon exception = {e.Message}, input = {polygonListString}");
                throw;
            }
        }

        private Dictionary<Point, IBooster> ParseBoosters(string boostersString)
        {
            try
            {
                var result = new Dictionary<Point, IBooster>();
                
                if (string.IsNullOrEmpty(boostersString))
                    return new Dictionary<Point, IBooster>();

                var parts = boostersString.Split(POLYGON_DELIMETER);

                foreach (var part in parts)
                {
                    var keyValuePair = ParseBooster(part);
                    if (!result.ContainsKey(keyValuePair.Key))
                        result.Add(keyValuePair.Key,keyValuePair.Value);
                }
                
                return result;


            }
            catch (Exception e)
            {
                Console.WriteLine($"parse boosters exception = {e.Message}, input = {boostersString}");
                throw;
            }
        }

        private KeyValuePair<Point, IBooster> ParseBooster(string boosterString)
        {
            try
            {
                if (string.IsNullOrEmpty(boosterString))
                    throw new Exception("booster must be not empty");
                
                var point = ParsePoint(boosterString.Substring(1, boosterString.Length-1));

                switch (boosterString[0])
                {
                    case 'B': return new KeyValuePair<Point, IBooster>(point,new ManipulatorExtension());
                    case 'F': return new KeyValuePair<Point, IBooster>(point,new FastWheels());
                    case 'L': return new KeyValuePair<Point, IBooster>(point,new Drill());
                    case 'R': return new KeyValuePair<Point, IBooster>(point,new TeleportBooster());
                    case 'C': return new KeyValuePair<Point, IBooster>(point,new CloneBooster());
                    case 'X': return new KeyValuePair<Point, IBooster>(point,new XBooster());
                    default:
                      throw new Exception($"unknown booster type {boosterString[0]}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"parse booster exception = {e.Message}, input = {boosterString}");
                throw;
            }
        }

    }
}