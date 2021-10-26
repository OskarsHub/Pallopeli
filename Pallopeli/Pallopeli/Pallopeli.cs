
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class Pallopeli : PhysicsGame
{
    Vector nopeusVasen = new Vector(-200, 0);
    Vector nopeusOikea = new Vector(200, 0);

    PhysicsObject pelipallo;
    PhysicsObject kauha;
    public override void Begin()
    {
        LuoKenttä();
        AsetaOhjaimet();

        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Pelin alkuvalikko", "Aloita peli", "Lopeta");
        alkuvalikko.AddItemHandler(1, Exit);
        alkuvalikko.AddItemHandler(0, AloitaPeli);
        Add(alkuvalikko);
    }

    public void LuoKenttä()
    {
        Level.CreateBorders(1.0, false);

        Level.Background.Color = Color.Black;

        //Kuva TitleMappiin
        ColorTileMap ruudut = ColorTileMap.FromLevelAsset("kentta");

        //Pikselien värit
        ruudut.SetTileMethod(Color.Black, LuoPallo);

        //luo kentän
        ruudut.Execute(34.2, 25);

        void LuoPallo(Vector paikka, double leveys, double korkeus)
            {
                PhysicsObject pallo = PhysicsObject.CreateStaticObject(40, 40);
                pallo.Shape = Shape.Circle;
                pallo.Color = Color.Gray;
                pallo.Position = paikka;
                Add(pallo);
            }

        pelipallo = LuoPelipallo(0.0, Level.Bottom + 150);
        kauha = LuoKauha(0.0, Level.Bottom + 10);

        Camera.ZoomToLevel();

    }

    PhysicsObject LuoKauha(double x, double y)
    {
        kauha = PhysicsObject.CreateStaticObject(100, 15);
        kauha.Shape = Shape.Rectangle;
        kauha.Color = Color.Gray;
        kauha.X = x;
        kauha.Y = y;
        kauha.Restitution = 1.0;
        Add(kauha);
        return kauha;
    }

    PhysicsObject LuoPelipallo(double x, double y)
    {
        pelipallo = PhysicsObject.CreateStaticObject(15, 15);
        pelipallo.Shape = Shape.Circle;
        pelipallo.Color = Color.White;
        pelipallo.Restitution = 1.0;
        pelipallo.Mass = 1.0;
        pelipallo.X = x;
        pelipallo.Y = y;
        Add(pelipallo);
        return pelipallo;
    }

    void AloitaPeli()
    {
        Vector impulssi = new Vector(500.0, 0.0);
        pelipallo.Hit(impulssi * pelipallo.Mass);
    }

    public void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.A, ButtonState.Down, LiikutaKauhaa, "Liikuta kauhaa vasemmalle", kauha, nopeusVasen);
        Keyboard.Listen(Key.A, ButtonState.Released, LiikutaKauhaa, null, kauha, Vector.Zero);
        Keyboard.Listen(Key.D, ButtonState.Down, LiikutaKauhaa, "liikuta kauhaa oikealle", kauha, nopeusOikea);
        Keyboard.Listen(Key.D, ButtonState.Released, LiikutaKauhaa, null, kauha, Vector.Zero);

        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    void LiikutaKauhaa(PhysicsObject kauha, Vector nopeus)
    {
        if ((nopeus.X < 0) && (kauha.Left < Level.Left))
        {
            kauha.Velocity = Vector.Zero;
            return;
        }
        if ((nopeus.X > 0) && (kauha.Right > Level.Right))
        {
            kauha.Velocity = Vector.Zero;
            return;
        }

        kauha.Velocity = nopeus;
    }

}


