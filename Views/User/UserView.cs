using System;
using spmserp.Controllers;
using spmserp.Entity;

namespace spmserp.View
{
    public class UserView:IUserView
    {
        private IUserController userController;
        Account loggedInAccount;
        public UserView()
        {
            userController = new UserController();
        }
        public void GenerateUserMenu()
        {
            Account login = userController.Login();
            while (true) {
                Console.WriteLine("");
                var choice = Convert.ToInt32(Console.ReadLine());
                switch (choice){
                    case 1:
                        userController.Deposit(login);
                        break;
                    case 2:
                        userController.WithDraw(login);
                        break;
                    case 3:
                        userController.Transfer(login);
                        break;
                    case 4:
                        userController.CheckBalance(login);
                        break;
                    case 5:
                        userController.UpdateInformation(login);
                        break;
                    case 6:
                        userController.UpdatePassword(login);
                        break;
                    case 7:
                        userController.CheckTransactionHistory(login);
                        break;
                }
                if (choice == 8){
                    Console.WriteLine("");
                    Environment.Exit(0);
                }
            }
        }
    }
}