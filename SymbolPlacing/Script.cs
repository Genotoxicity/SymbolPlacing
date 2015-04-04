using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ProELib;

namespace SymbolPlacing
{
    class Script
    {
        private int processId;
        private CharacteristicsWindow window;

        public Script(CharacteristicsWindow window, int processId)
        {
            this.processId = processId;
            this.window = window;
        }

        public void PlaceSymbols()
        {
            E3Project project = new E3Project(processId);
            try
            {
                int deviceId = GetSelectedDeviceId(project);
                int sheetId = GetActiveSheetId(project);
                if (deviceId != 0 && sheetId != 0)
                {
                    List<PlaceSymbol> symbols = GetSymbols(project, deviceId, SymbolReturnParameter.All);
                    PlaceSymbol leftSymbol = symbols.First();
                    symbols.ForEach(s => s.Calculate());
                    symbols.ForEach(s => s.Delete());
                    Point position = leftSymbol.PlaceInteractively();
                    if (leftSymbol.IsPlaced)
                    {
                        Sheet sheet = project.Sheet;
                        sheet.Id = sheetId;
                        if (IsInBoundaries(sheet, position, symbols))
                        {
                            double x = sheet.MoveRight(position.X, leftSymbol.Margins.Right);
                            double top = sheet.MoveUp(position.Y, leftSymbol.Margins.Top);
                            for (int i = 1; i < symbols.Count; i++)
                            {
                                PlaceSymbol symbol = symbols[i];
                                x = sheet.MoveRight(x, symbol.Margins.Left);
                                double y = sheet.MoveDown(top, symbol.Margins.Top);
                                symbol.Place(sheetId, x, y);
                                symbol.SetCharacteristic(0);
                                x = sheet.MoveRight(x, symbol.Margins.Right);
                            }
                            SetSymbolsCharacteristic(project, symbols);
                        }
                        else
                        {
                            leftSymbol.Delete();
                            MessageBox.Show("Недостаточно места для размещения символов", "Ошибка");
                            window.Hide();
                        }
                    }

                }
            }
            finally
            {
                project.Release();
            }
        }

        private static List<PlaceSymbol> GetSymbols(E3Project project, int deviceId, SymbolReturnParameter parameter)
        {
            NormalDevice device = project.NormalDevice;
            device.Id = deviceId;
            List<int> symbolIds = device.GetSymbolIds(parameter);
            List<PlaceSymbol> symbols = new List<PlaceSymbol>(symbolIds.Count);
            symbolIds.ForEach(sId => symbols.Add(new PlaceSymbol(project, sId)));
            List<string> types = new List<string>(3) { "RJ45", "SFPMiniGBIC", "DualPersonalityPort" };
            symbols.RemoveAll(ps => !types.Contains(ps.Type));
            if (symbols.Count > 0)
            {
                symbols.First().SetPosition(SymbolPosition.Left);
                symbols.Last().SetPosition(SymbolPosition.Right);
                for (int i = 1; i < symbols.Count - 1; i++)
                    symbols[i].SetPosition(SymbolPosition.Center);
            }
            return symbols;
        }

        private bool IsInBoundaries(Sheet sheet, Point position, List<PlaceSymbol> placeSymbols)
        {
            Margins commonMargins = GetCommonMargins(placeSymbols);
            double left = sheet.MoveLeft(position.X, commonMargins.Left);
            double right = sheet.MoveRight(position.X, commonMargins.Right);
            double top = sheet.MoveUp(position.Y, commonMargins.Top);
            double bottom = sheet.MoveDown(position.Y, commonMargins.Bottom);
            bool isToTheRightFromLeftEdge = sheet.IsToTheRight(sheet.DrawingArea.Left, left);
            bool isToTheLeftFromRightEdge = sheet.IsToTheLeft(sheet.DrawingArea.Right, right);
            bool isAboveBottomEdge = sheet.IsAbove(sheet.DrawingArea.Bottom, bottom);
            bool isBelowTopEdge = sheet.IsBelow(sheet.DrawingArea.Top, top);
            return isToTheLeftFromRightEdge && isToTheRightFromLeftEdge && isAboveBottomEdge && isBelowTopEdge;
        }

        private int GetSelectedDeviceId(E3Project project)
        {
            List<int> ids = project.SelectedAllDeviceIds;
            ids.AddRange(project.TreeSelectedAllDeiveIds);
            ids = ids.Distinct().ToList();
            ids.Remove(0);
            if (ids.Count==0)
            {
                MessageBox.Show("Не выбрано ни одного изделия", "Ошибка");
                return 0;
            }
            if (ids.Count>1)
            {
                MessageBox.Show("Выбрано несколько изделий", "Ошибка");
                return 0;
            }
            return ids.First();
        }

        private int GetActiveSheetId(E3Project project)
        {
            int sheetId = project.ActiveSheetId;
            if (sheetId == 0)
                MessageBox.Show("Не открыт лист для размещения символа", "Ошибка");
            return sheetId;
        }

        private Margins GetCommonMargins(List<PlaceSymbol> placeSymbols)
        {
            double left = placeSymbols.First().Margins.Left;
            double right = placeSymbols.Sum(ps => ps.Size.Width) - left;
            double top = placeSymbols.Max(ps => ps.Margins.Top);
            double bottom = placeSymbols.Max(ps => ps.Margins.Bottom);
            return new Margins(left, right, top, bottom);
        }

        public void SetCharacteristics()
        {
            E3Project project = new E3Project(processId);
            try
            {
                int deviceId = GetSelectedDeviceId(project);
                if (deviceId != 0)
                {
                    List<PlaceSymbol> symbols = GetSymbols(project, deviceId, SymbolReturnParameter.Placed);
                    if (symbols.Count == 0)
                    {
                        MessageBox.Show("У выбранного изделия нет размещенных символов", "Ошибка");
                        project.Release();
                        return;
                    }
                    SetSymbolsCharacteristic(project, symbols);
                }
            }
            finally
            {
                project.Release();
            }
        }

        private void SetSymbolsCharacteristic(E3Project project, List<PlaceSymbol> symbols)
        {
            window.Display(symbols);
        }
    }
}
