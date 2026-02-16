using System.Collections.Generic;

public class LoadController
{
    private List<IController> controllers;

    public LoadController()
    {
        controllers = new List<IController>();
        var keyboard = new KeyboardController();

    
    }
}