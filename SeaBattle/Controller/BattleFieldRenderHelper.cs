using System.Collections.Generic;
using System.Linq;
using SeaBattle.Controller.View;
using SeaBattle.Logger;
using SeaBattle.Model.Dto;
using SeaBattle.Service;
using Spectre.Console;

namespace SeaBattle.Controller
{
    class BattleFieldRenderHelper
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<BattleFieldRenderHelper>();
        private static readonly BattleFieldService.FieldMarkup markup =
            new('o', '#', '~', 'D', 'X');
        private readonly BattleFieldService battleFieldService;

        private static readonly Dictionary<char, string> markupDecorator =
            new Dictionary<char, string>
            {
                {markup.MissShot,$"[grey70]{markup.MissShot}[/]"},
                {markup.HitShot,$"[orange3]{markup.HitShot}[/]"},
                {markup.NoShot,$"[navy]{markup.NoShot}[/]"},
                {markup.ShipDeck,$"[green]{markup.ShipDeck}[/]"},
                {markup.DefeatShipDeck,$"[maroon]{markup.DefeatShipDeck}[/]"},
            };

        public BattleFieldRenderHelper(BattleFieldService battleFieldService)
        {
            this.battleFieldService = battleFieldService;
        }

        public Table CreateField(BattleFieldDto battleField, bool isEnemy)
        {
            log.Debug("Creating battlefield");
            var renderedField = battleFieldService.RenderField(battleField, markup, isEnemy);
            Table field = new Table();
            field.Border(TableBorder.None);

            field.AddColumns(" ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J");
            for (int i = 0; i < renderedField.Length; i++)
            {
                var row = renderedField[i];
                var strArr = new List<string>
                {
                    (i + 1).ToString()
                };
                strArr.AddRange(row.Select(ch => markupDecorator.GetValueOrDefault(ch,"")));
                field.AddRow(strArr.ToArray());
            }

            return field;
        }

        public Table CreateFields(GameDto game)
        {
            log.Debug("Creating battlefields table");
            var userField = game.BattleFields
                .Find(bf => bf.Owner.Id == game.CurrentUser.Id);
            var enemyField = game.BattleFields
                .Find(bf => bf.Owner.Id != game.CurrentUser.Id);
            var table = new Table();
            table.AddColumns($"{userField.Owner.Name} Field", $"{enemyField.Owner.Name} field");
            table.AddRow(
                CreateField(userField, false),
                CreateField(enemyField, game.Winner is null)
            );
            return table;
        }
    }
}
