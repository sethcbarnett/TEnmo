﻿using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;
using TenmoServer.Models;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                decimal Balance = tenmoApiService.GetBalance(tenmoApiService.UserId);
                Console.WriteLine($"Your current balance:{Balance:C2}");
                console.Pause();
            }

            if (menuSelection == 2)
            {
                // View your past transfers
                IList<Transfer> transfers = tenmoApiService.GetAllTransfers(tenmoApiService.UserId);
                console.PrintTransfers(transfers);

                Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");               
                Console.ReadLine();
          
                console.Pause();
            }

            if (menuSelection == 3)
            {
                // View your pending requests
                IList<Transfer> transfers = tenmoApiService.GetAllTransfers(tenmoApiService.UserId);
                console.PrintTransfers(transfers);
                console.Pause();
                
            }

            if (menuSelection == 4)
            {
                // Send TE bucks
                //decimal UserId = tenmoApiService.GetTransfer(tenmoApiService.UserId);
                //decimal Balance = tenmoApiService.GetTransfer(tenmoApiService.UserId);
                IList<Transfer> transfers = tenmoApiService.GetAllTransfers(tenmoApiService.UserId);
                console.PrintSendingBucks(transfers);
                decimal Balance = tenmoApiService.GetBalance(tenmoApiService.UserId);
                Console.WriteLine($"Your current balance:{Balance:C2}");
                Console.WriteLine("Id of the user you are sending to:");
                //decimal ToUserId = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter amount to send:");
                //decimal Amount = decimal.Parse(Console.ReadLine());
                console.Pause();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
                IList<Transfer> transfers = tenmoApiService.GetAllTransfers(tenmoApiService.UserId);
                console.PrintRequestingBucks(transfers);
                decimal Balance = tenmoApiService.GetBalance(tenmoApiService.UserId);
                Console.WriteLine($"Your current balance:{Balance:C2}");
                Console.WriteLine("Id of the user you are requesting from[0]:");
                //decimal fromUserId = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter amount to request:");
                //decimal Amount = decimal.Parse(Console.ReadLine());
                console.Pause();
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }


    }
}
