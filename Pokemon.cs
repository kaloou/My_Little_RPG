using System;
using System.Collections.Generic;
using System.Text;


/*•	id : ID PokéAPI
•	nom : Nom du Pokémon
•	pointsVieBase : HP ÷ 3 (minimum 10)
•	forceBase : Attaque ÷ 8 (minimum 5)
•	defenseBase : Défense ÷ 8 (minimum 3)
•	experienceBase : Moyenne des stats × 2
•	spriteUrl : URL du sprite front_default
•	type1 : Premier type du Pokémon
•	type2 : Second type du Pokémon
*/
namespace My_Little_RPG

{
    internal class Pokemon
    {
        public int ID { get; set; }
        public string Nom { get; set; }
        public int HpBase { get; set; }
        public int ForceBase { get; set; }
        public int DefenseBase { get; set; }
        public int ExperienceBase { get; set; }
        public string URL { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }

        public Pokemon(int id, string nom, int hp, int force, int defense, string url, string type1)
        {
            ID = id;
            Nom = nom;
            HpBase = (hp != 0 && hp / 3 >= 10) ? hp / 3 : 10;
            ForceBase = (force != 0 && force / 8 >= 5) ? force / 8 : 5;
            DefenseBase = (defense != 0 && defense / 8 >= 3) ? defense / 8 : 3;
            ExperienceBase = (HpBase + DefenseBase + ForceBase) / 3;
            URL = url;
            Type1 = type1;

        }
        public Pokemon(int id, string nom, int hp, int forceBase, int defenseBase, string url, string type1, string type2)
            : this (id, nom, hp,  forceBase,  defenseBase,  url, type1)
        {
            Type2 = type2;
        }

        

    }
}
