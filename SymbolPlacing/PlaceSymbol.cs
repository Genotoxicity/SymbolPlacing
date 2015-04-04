using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ProELib;

namespace SymbolPlacing
{
    public class PlaceSymbol
    {
        private SymbolPosition position;
        private List<string> availableCharacteristics;
        private int id;
        //private int processId;
        private Symbol symbol;

        public string Type { get; private set; }

        public string Characteristic
        {
            get
            {
                symbol.Id= id;
                return symbol.Characteristic;
            }
        }

        public Margins Margins { get; private set; }

        public bool IsPlaced { get; private set; }

        public string Name
        {
            get
            {
                symbol.Id = id;
                return symbol.Name;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public PlaceSymbol(E3Project project, int id)
        {
            this.id = id;
            symbol = project.Symbol;
            symbol.Id = id;
            Type = symbol.SymbolTypeNameWithoutCharacteristic;
        }

        public Size Size { get; private set; }

        public void Calculate()
        {
            symbol.Id = id;
            Area area = symbol.Area;
            Margins = GetMargins(area);
            Size = new Size(area.Width, area.Height);
        }

        public void Delete()
        {
            symbol.Id = id;
            symbol.Delete();
        }

        public void SetCharacteristic(int index)
        {
            if (availableCharacteristics.Count > 0)
            {
                symbol.Id = id;
                symbol.Characteristic = availableCharacteristics[index];
            }
        }

        public void SetPosition(SymbolPosition position)
        {
            this.position = position;
            symbol.Id = id;
            switch (position)
            {
                case SymbolPosition.Center:
                    availableCharacteristics = new List<string>(4) { "4. Проходной", "4.1 Проходной Error", "4.2 Проходной подключен", "4.3 Проходной отключен программно" };
                    break;
                case SymbolPosition.Left:
                    availableCharacteristics = new List<string>(4) { "2. Левый", "2.1 Левый Error", "2.2 Левый подключен", "2.3 Левый отключен программно" };
                    break;
                case SymbolPosition.Right:
                    availableCharacteristics = new List<string>(4) { "3. Правый", "3.1 Правый Error", "3.2 Правый подключен", "3.3 Правый отключен программно" };
                    break;
                default:
                    availableCharacteristics = new List<string>(0);
                    break;
            }
        }

        private Margins GetMargins(Area area)
        {
            double left, right, top, bottom;
            left = 0-area.Left;
            right = area.Right;
            top = area.Top;
            bottom = 0 - area.Bottom;
            return new Margins(left, right, top, bottom);
        }

        public Point PlaceInteractively()
        {
            symbol.Id = id;
            symbol.PlaceInteractively();
            IsPlaced = symbol.SheetId > 0;
            return symbol.Position;
        }

        public void Place(int sheetId, double x, double y)
        {
            symbol.Id = id;
            symbol.Place(sheetId, x, y);
        }

        public int GetStateIndex()
        {
            for (int i = 0; i < availableCharacteristics.Count; i++)
                if (availableCharacteristics[i].Equals(Characteristic))
                    return i;
            return -1;
        }
    }
}
