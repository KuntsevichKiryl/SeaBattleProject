using System.Collections.Generic;
using SeaBattle.Model;
using SeaBattle.Model.Domain;
using System.Linq;
using SeaBattle.Controller.View;
using SeaBattle.Logger;
using SeaBattle.Model.Dto;

namespace SeaBattle.Service
{
    class BattleFieldService
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<BattleFieldService>();
        private readonly Dictionary<int, int> shipCount = new()
        {
            { ShipType.OneDecker.Length, ShipType.OneDecker.MaxCount },
            { ShipType.TwoDecker.Length, ShipType.TwoDecker.MaxCount },
            { ShipType.ThreeDecker.Length, ShipType.ThreeDecker.MaxCount },
            { ShipType.FourDecker.Length, ShipType.FourDecker.MaxCount },
        };

        private static int FieldSize => 10;

        public bool AddShip(BattleFieldDto battleField, int x, int y, ShipType shipType, ShipRotation shipRotation)
        {
            log.Debug("Adding ship");
            Ship ship = new()
            {
                Length = shipType.Length
            };

            SetHeadAndTail(ship, x, y, shipRotation);
            var valid = IsValidShipPlacement(battleField, ship);
            if (!valid)
            {
                return false;
            }
            battleField.Ships.Add(ship);

            return true;
        }

        public ShotResult AddShot(BattleFieldDto battleField, int x, int y)
        {
            log.Debug("Adding shot");
            bool isHit = false;
            bool isDestroyed = false;
            ShotResult shotResult = ShotResult.Miss;
            foreach (Ship ship in battleField.Ships)
            {
                if (ship.XHead <= x && ship.XTail >= x
                                    && ship.YHead <= y && ship.YTail >= y)
                {
                    isHit = true;
                    ship.MakeWound();
                    isDestroyed = ship.IsDestroyed();
                    log.Debug("Ship destroyed");
                    break;
                }
            }

            Shot shot = new Shot();
            shot.X = x;
            shot.Y = y;
            shot.IsHit = isHit;
            battleField.Shots.Add(shot);

            switch (isHit)
            {
                case true when isDestroyed:
                    shotResult = ShotResult.Destroy;
                    break;
                case true:
                    shotResult = ShotResult.Hit;
                    break;
            }

            return shotResult;
        }

        public bool IsDefeated(BattleFieldDto battleField)
        {
            return battleField.Ships.All(ship => ship.IsDestroyed());
        }

        public char[][] RenderField(BattleFieldDto battleField, FieldMarkup markup, bool hideShipDeck = true)
        {
            log.Debug("Rendering field");
            char[][] field = new char[FieldSize][];

            for (int i = 0; i < FieldSize; i++)
            {
                field[i] = new char[FieldSize];
                for (int j = 0; j < FieldSize; j++)
                {
                    field[i][j] = markup.NoShot;
                }
            }

            foreach (Ship ship in battleField.Ships)
            {
                if (!hideShipDeck || ship.IsDestroyed())
                {
                    for (int i = ship.YHead; i <= ship.YTail; i++)
                    {
                        for (int j = ship.XHead; j <= ship.XTail; j++)
                        {
                            field[i][j] = ship.IsDestroyed() ? markup.DefeatShipDeck : markup.ShipDeck;
                        }
                    }
                }
            }

            foreach (Shot shot in battleField.Shots)
            {
                var currentFill = field[shot.Y][shot.X];
                if (currentFill == markup.DefeatShipDeck)
                {
                    continue;
                }
                field[shot.Y][shot.X] = shot.IsHit ? markup.HitShot : markup.MissShot;
            }

            return field;
        }

        public Dictionary<int, int> RestToFill(BattleFieldDto battleField)
        {
            Dictionary<int, int> shipCountRest = new Dictionary<int, int>();

            var currentShipsCount = battleField.Ships
                .GroupBy(ship => ship.Length)
                .ToDictionary(pair => pair.Key, pair => pair.ToList().Count);

            foreach (var (key, value) in shipCount)
            {
                var count = currentShipsCount.GetValueOrDefault(key);
                if (value - count != 0)
                {
                    shipCountRest.Add(key, value - count);
                }
            }

            return shipCountRest;
        }

        private void SetHeadAndTail(Ship target, int x, int y, ShipRotation rotation)
        {
            log.Debug("Calculating ship position");
            switch (rotation)
            {
                case ShipRotation.Left:
                    target.XHead = x - (target.Length - 1);
                    target.YHead = y;
                    target.XTail = x;
                    target.YTail = y;
                    break;
                case ShipRotation.Right:
                    target.XHead = x;
                    target.YHead = y;
                    target.XTail = x + (target.Length - 1);
                    target.YTail = y;
                    break;
                case ShipRotation.Up:
                    target.XHead = x;
                    target.YHead = y - (target.Length - 1);
                    target.XTail = x;
                    target.YTail = y;
                    break;
                case ShipRotation.Down:
                    target.XHead = x;
                    target.YHead = y;
                    target.XTail = x;
                    target.YTail = y + (target.Length - 1);
                    break;
            }
        }

        private bool IsValidShipPlacement(BattleFieldDto battleField, Ship ship)
        {
            log.Debug("Ship position validation");
            if (ship.XHead < 0 || ship.XTail >= FieldSize)
                return false;

            if (ship.YHead < 0 || ship.YTail >= FieldSize)
                return false;

            var startXCheck = ship.XHead - 1;
            var startYCheck = ship.YHead - 1;
            var deltaXCheck = ship.XTail - ship.XHead + 3;
            var deltaYCheck = ship.YTail - ship.YHead + 3;
            var xRange = Enumerable.Range(startXCheck, deltaXCheck).ToArray();
            var yRange = Enumerable.Range(startYCheck, deltaYCheck).ToArray();

            foreach (var placedShip in battleField.Ships)
            {
                foreach (var x in xRange)
                {
                    foreach (var y in yRange)
                    {
                        if (placedShip.XHead <= x && placedShip.XTail >= x
                                                  && placedShip.YHead <= y && placedShip.YTail >= y)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public record FieldMarkup(char MissShot, char HitShot, char NoShot, char ShipDeck, char DefeatShipDeck);
    }
}
