﻿function main()
{
    try
    {
        var Manager_b = new SCEELibs.Editor.ConsoleManager();

        var p = 0, n = 5, u_result = 2;

        for (var k = p; k <= n; k++)
        {
            u_result = u_result + (u_result + 2);
        }
        Manager_b.sendConsoleInformationNotification("Résultat: " + u_result);

        //Manager = SerrisModulesServer.Items.Lol;
        //Test = Windows.UI.Popups.MessageDialog("test");

        /*var uri = new Windows.UI.Popups.MessageDialog("Bonjour !");
        uri.title = "Ceci est un titre c:";
        uri.showAsync();*/

        var Manager = new SCEELibs.Editor.SheetManager();
        //var button = new Windows.UI.Xaml.Controls.Button(); button.content = "coucou !";

        Manager.createNewSheet("Bouton test", "HTML/content.html", currentID);

        //Manager.deleteModule(1);

        //global = JavaScriptValue.GlobalObject;

       /* var U = 0, m; //Définition des variables

        for (m = 1; U <= 5; m++) //Définition des paramètres de la boucle for ("pour")
        {
            U += 1 / m;
        }

        console.log("U est supérieur ou égal à 5 à partir de " + m); //Affichage du résultat dans la console
        */
    }
    catch (e)
    {
        console.log(e.message);
    }
}