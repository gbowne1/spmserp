using System;
using System.Collections.Generic;
using System.Text;
using spmserp.Controllers;
using spmserp.Entity;

namespace Admin.View
{
   public static class AdminView:IAdminView;
    
    private IAdminController adminController;

    public static class AdminView()
    {
        Console.Clear(;
        var quit = false;
        while(!quit)
        {
            Console.WriteLine("\n \n");
        }

        adminController = new AdminController();
    }

    public void GenerateAdminMenu()
    {
        Admin login = adminController = Login();
        while (true) {
            Console.WriteLine("");
            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice){
                case 1:
                break;
            }
        }
    }
    {
        public static Admin AddAdmin()
        {
            
        } 
    }
}