﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using TheBrand;
using TheBrand.ComboSystem;
using TheBrand.Commons;

namespace TheEkko
{
    class Ekko : IMainContext
    {
        private Menu _mainMenu;
        private Orbwalking.Orbwalker _orbwalker;
        private ComboProvider _comboProvider;
        private MenuItem _drawR, _drawQ, _drawQEx;

        public void Load(EventArgs eArgs)
        {
            _comboProvider = new ComboProvider(new Skill[] { new EkkoQ(new Spell(SpellSlot.Q)), new EkkoW(new Spell(SpellSlot.W)), new EkkoE(new Spell(SpellSlot.E)), new EkkoR(new Spell(SpellSlot.R)) }.ToList(), 1000);

            _mainMenu = CreateMenu("The Ekko", true);
            var orbwalkerMenu = CreateMenu("Orbwalker", _mainMenu);
            var targetSelectorMenu = CreateMenu("Target Selector", _mainMenu);
            var comboMenu = CreateMenu("Combo", _mainMenu);
            var harassMenu = CreateMenu("Harass", _mainMenu);
            ManaManager.Initialize(_mainMenu, "Manamanager", true, false, false);
            IgniteManager.Initialize(_mainMenu);
            var drawingMenu = CreateMenu("Drawing", _mainMenu);

            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            TargetSelector.AddToMenu(targetSelectorMenu);

            comboMenu.AddMItem("Use Q", true, (sender, args) => _comboProvider.SetEnabled<EkkoQ>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.AddMItem("Use W", true, (sender, args) => _comboProvider.SetEnabled<EkkoW>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.AddMItem("Use E", true, (sender, args) => _comboProvider.SetEnabled<EkkoE>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.AddMItem("Use R", true, (sender, args) => _comboProvider.SetEnabled<EkkoR>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.ProcStoredValueChanged<bool>();
            comboMenu.AddMItem("Min Ult Enemies", new Slider(3, 1, HeroManager.Enemies.Count));
            comboMenu.AddMItem("Min Ult Health %", new Slider(30));
            comboMenu.AddMItem("Ult for Save", true);

            harassMenu.AddMItem("Use Q", true, (sender, args) => _comboProvider.SetEnabled<EkkoQ>(Orbwalking.OrbwalkingMode.Mixed, args.GetNewValue<bool>()));
            harassMenu.ProcStoredValueChanged<bool>();

            _drawQ = drawingMenu.AddMItem("Draw Q", new Circle(true, Color.OrangeRed));
            _drawQEx = drawingMenu.AddMItem("Draw Q Ex", new Circle(false, Color.Yellow));
            _drawR = drawingMenu.AddMItem("Draw R", new Circle(true, Color.Red));
            _mainMenu.AddToMainMenu();

            Game.OnUpdate += Update;
            Drawing.OnDraw += Draw;

            _comboProvider.Initialize(this);
        }

        private void Draw(EventArgs args)
        {
            var drawR = _drawR.GetValue<Circle>();
            var drawQ = _drawQ.GetValue<Circle>();
            var drawQEx = _drawQEx.GetValue<Circle>();

            if (drawR.Active)
            {
                var ekko = ObjectManager.Get<GameObject>().FirstOrDefault(item => item.Name == "Ekko_Base_R_TrailEnd.troy");
                if (ekko != null)
                    Render.Circle.DrawCircle(ekko.Position, 400, drawR.Color);
            }
            if (drawQ.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 700, drawQ.Color);
            }
            if (drawQEx.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1000, drawQEx.Color);
            }
        }

        private void Update(EventArgs args)
        {
            _comboProvider.Update(this);
        }

        private Menu CreateMenu(string name, Menu menu)
        {
            var newMenu = new Menu(name, name);
            menu.AddSubMenu(newMenu);
            return newMenu;
        }

        private Menu CreateMenu(string name, bool root = false)
        {
            return new Menu(name, name, root);
        }

        public Menu GetRootMenu()
        {
            return _mainMenu;
        }

        public Orbwalking.Orbwalker GetOrbwalker()
        {
            return _orbwalker;
        }
    }
}
