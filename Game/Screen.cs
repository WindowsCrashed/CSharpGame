﻿using System;
using System.IO;
using Game.Characters;
using Game.Characters.Inventory;
using Game.Miscellaneous;
using Game.GameLogic.Enums;

namespace Game
{
    class Screen      // Class containing screen related functions (EVENTUALLY MAKE AN INTERNAL SCREEN FUNCTION)
    {
        private Directories _directories = new Directories();
        private Character _opponent;
        private Player _player;

        public Screen(Player player, Character enemy)
        {
            _opponent = enemy;
            _player = player;
        }


        // ---------- LOGIC FUNCTIONS ----------------

        public void CenterText(string text)     // Center texts in screen
        {
            for (int i = 0; i < 53 - (text.Length / 2); i++)
            {
                Console.Write(" ");
            }
            Console.Write(text);
        }
        public void DottedLine()      // Prints lines to be used in messages
        {
            for (int i = 0; i < 106; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine("\n");
        }
        public void HealthBarLogic(Character c)  // Used for calculating the health bar length
        {
            int relativeHP = c.Hp * 2;      // Used for the printing

            for (int i = 1; i <= 20; i++)
            {
                if (relativeHP - i < 0)
                {
                    Console.Write("-");
                }
                else
                {
                    Console.Write("H");
                }
            }
        }


        // ----------- SPRITES -----------

        public void Sprite(string spriteName)  // (MAY BECOME USELESS [OR NOT...]) Prints any sprite in main directory
        {
            try
            {
                string[] lines = File.ReadAllLines(_directories.GetSpritePath(spriteName));

                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR 404");
                Console.WriteLine(e.Message);
            }
        }
        public void AttackSprite(Animation animation, Character user, string spriteName)  // Prints attack sprites only
        {
            try
            {
                string[] lines = File.ReadAllLines(_directories.GetAttackSpritePath(animation.ToString(), user.GetType().Name, spriteName));

                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR 404");
                Console.WriteLine(e.Message);
            }
        }
        public void DefenceSprite(string spriteName)  // Prints defence sprites only
        {
            try
            {
                string[] lines = File.ReadAllLines(_directories.GetDefenceSpritePath(spriteName));

                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR 404");
                Console.WriteLine(e.Message);
            }
        }
        public void IdlePose()   // (WIP) Prints idle pose or defence poses according to the given conditions
        {
            if (_player.ShieldUp && !_opponent.ShieldUp)
            {
                Sprite("DefencePosePlayer");
            } else if (!_player.ShieldUp && _opponent.ShieldUp)
            {
                Sprite("DefencePoseEnemy");
            }
            else if (_player.ShieldUp && _opponent.ShieldUp)
            {
                Sprite("DefencePoseBoth");
            } else
            {
                Sprite("Idle");
            }
        }
        public void PreparePoseLogic(Character user, int pos)   // Prints different prepare sprites according to the given conditions
        {
            if (user.Opponent.ShieldUp)
            {
                AttackSprite(user.MoveSet[pos].Animation, user, "PrepareES"); // ES == Enemy Shield
            }
            else
            {
                AttackSprite(user.MoveSet[pos].Animation, user, "Prepare");
            }
        }


        // ---------- PRINTING FUNCTIONS -------------

        // ---- GENERAL ----
        public void Message(string text)      // Prints centered text in-between lines
        {
            DottedLine();
            CenterText($"{text}\n\n");
            DottedLine();
        }
        public void MessageDouble(string line1, string line2) // Prints message with two lines of text
        {
            DottedLine();
            CenterText($"{line1}\n");
            CenterText($"{line2}\n\n");
            DottedLine();
        }
        public void CombatUI(string text)   // Prints basic combat UI
        {
            HealthBar();
            Message(text);
        }
        public void HealthBar()   // Prints the health bars of player and opponent
        {
            Console.WriteLine();

            for (int i = 0; i < 14; i++)
            {
                Console.Write(" ");
            }

            HealthBarLogic(_player);

            for (int i = 0; i < 36; i++)
            {
                Console.Write(" ");
            }

            HealthBarLogic(_opponent);

            Console.WriteLine("\n");
        }
        

        // ---- PRE-GAME ----
        public void Encounter()     // Prints the encounter message and decisions 
        {
            Sprite("Encounter");
            
            Message($"A { _opponent.Name.ToUpper()} STEPS INTO YOUR PATH!!!");

            CenterText("What are you going to do?\n\n");
            CenterText("[1] FIGHT    [2] TURN BACK\n\n");
        }
        public void PrepareFight()      // Prints the first option's message
        {
            Message("Taking the sword and preparing to fight!");
        }
        

        // ---- TURNS ----
        public void PlayerTurn()     // Prints the main UI
        {
            IdlePose();
            
            CombatUI("What is going to be your next move?");
            
            CenterText("[1] ATTACK    [2] DEFEND\n");
            CenterText("[3] INVENTORY    [4] FLEE\n\n");
        }
        public void EnemyTurn()   // Prints the Enemy Turn UI
        {
            IdlePose();

            CombatUI($"The {_opponent.Name} is preparing to move...");
        }
        
        

        // ---- COMBAT ----
        public void PlayerAttackMenu()     // Prints the Attack Choice UI
        {
            IdlePose();

            CombatUI("Which attack are you going to use?");

            CenterText($"[1] {_player.MoveSet[0].Name}    [2] {_player.MoveSet[1].Name}     [0] RETURN\n");
            CenterText($"[3] {_player.MoveSet[2].Name}    [4] {_player.MoveSet[3].Name}\n\n");
        }
        public void PrepareAttack(Character user, int pos)  // Prints attack preparation UI
        {
            PreparePoseLogic(user, pos);

            CombatUI($"{user.MoveSet[pos].Name.ToUpper()} INCOMING!!!");
        }
        public void MakeAttack(Character user, int pos)    // Prints damage output UI
        {
            AttackSprite(user.MoveSet[pos].Animation, user, "Strike");

            CombatUI($"{user.MoveSet[pos].Damage} DAMAGE!!!");
        }
        public void Defence(Character defender)     // Prints Shield Status
        {
            IdlePose();

            if (defender.GetType().Name == "Player")
            {
                CombatUI("Shield raised!!!");
            } else
            {
                CombatUI($"The {_opponent.Name} raised his shield!!!");
            }
        }
        public void BlockAttack(Character defender)    // Prints block message
        {
            DefenceSprite(defender.GetType().Name + "Block");

            CombatUI("ATTACK BLOCKED!!!");
        }
        

        // ---- INVENTORY ----
        public void PlayerInventory(CharInventory inventory)     // Prints Inventory choice UI
        {
            IdlePose();

            CombatUI("Which item are you going to use?");

            CenterText($"[1] {inventory.CheckItemSlot(0)}    [2] {inventory.CheckItemSlot(1)}     [0] RETURN\n");
            CenterText($"[3] {inventory.CheckItemSlot(2)}    [4] {inventory.CheckItemSlot(3)}\n\n");
        }
        public void UseItem(CharInventory inventory, int pos)   // Prints Used item message
        {
            IdlePose();

            CombatUI($"{inventory.Owner.Name} have used {inventory.Items[pos].Name}!!!");
        }
        public void HealthPotion(int healingPoints)   // Prints health potion message
        {
            IdlePose();

            CombatUI($"{healingPoints} HEALTH POINTS RESTORED!!!");
        }


        // ---- END GAME ----
        public void Flee()     // Prints the quit message
        {
            Message("Turning back and seeking another path...");
        }
        public void Victory()   // Prints the victory message
        {
            MessageDouble("Enemy defeated!", "YOU WIN!!!");
        }
        public void Defeat()    // Prints the defeat message
        {
            MessageDouble("You've been defeated...", "YOU LOSE...");
        }


        // ---- FIX LATER ----
        public void EnemyAttack(int pos)   // (REMOVE AFTER MAKING ANIMATIONS FOR ENEMY)
        {
            IdlePose();

            CombatUI($"{_opponent.MoveSet[pos].Name.ToUpper()} INCOMING!!!");
        }
        public void Damage(Character c, int pos)    // (REMOVE AFTER MAKING ANIMATIONS FOR ENEMY)
        {
            DottedLine();
            CenterText($"{c.MoveSet[pos].Damage} DAMAGE!!!\n\n");
            DottedLine();
        }
    }
}
