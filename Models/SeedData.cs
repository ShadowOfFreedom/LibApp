using LibApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace LibApp.Models {
    public static class SeedData {
        public static void Initialize(IServiceProvider serviceProvider) {
            using var context =
                new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Database.CanConnect()) {
                if (!context.MembershipTypes.Any()) {
                    context.MembershipTypes.AddRange(
                        new MembershipType {
                            Id = 1,
                            Name = "Pay as You Go",
                            SignUpFee = 0,
                            DurationInMonths = 0,
                            DiscountRate = 0
                        },
                        new MembershipType {
                            Id = 2,
                            Name = "Monthly",
                            SignUpFee = 30,
                            DurationInMonths = 1,
                            DiscountRate = 10
                        },
                        new MembershipType {
                            Id = 3,
                            Name = "Quaterly",
                            SignUpFee = 90,
                            DurationInMonths = 3,
                            DiscountRate = 15
                        },
                        new MembershipType {
                            Id = 4,
                            Name = "Yearly",
                            SignUpFee = 300,
                            DurationInMonths = 12,
                            DiscountRate = 20
                        });

                    context.SaveChanges();
                }


                if (!context.Roles.Any()) {
                    context.Roles.AddRange(GetRoles());
                    context.SaveChanges();
                }
            }
        }

        private static IEnumerable<Role> GetRoles() {
            return new List<Role> {
                new() {Name = "User"},
                new() {Name = "StoreManager"},
                new() {Name = "Owner"}
            };
        }

        [Conditional("DEBUG")]
        public static void ClearDebugSeed(IServiceProvider serviceProvider) {
            using var context =
                new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Rentals.Any())
                context.Rentals.RemoveRange(context.Rentals);

            if (context.Customers.Any())
                context.Customers.RemoveRange(context.Customers);

            if (context.Books.Any())
                context.Books.RemoveRange(context.Books);

            context.SaveChanges();
        }


        [Conditional("DEBUG")]
        public static void DebugInitialize(IServiceProvider serviceProvider) {
            using var context =
                new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Books.Any())
                return;
            
            context.Books.AddRange(
                new Book {
                    Name = "Pride and Prejudice",
                    AuthorName = "Jane Austen",
                    GenreId = 3,
                    DateAdded = DateTime.UtcNow,
                    ReleaseDate = new DateTime(1813,1,28),
                    NumberInStock = 20,
                    NumberAvailable = 20
                },
                new Book {
                    Name = "Hamlet",
                    AuthorName = "William Shakespeare",
                    GenreId = 3,
                    DateAdded = DateTime.UtcNow,
                    ReleaseDate = new DateTime(1601),
                    NumberInStock = 20,
                    NumberAvailable = 20
                },
                new Book {
                    Name = "Ubik",
                    AuthorName = "Philip K. Dick",
                    GenreId = 7,
                    DateAdded = DateTime.UtcNow,
                    ReleaseDate = new DateTime(1961),
                    NumberInStock = 5,
                    NumberAvailable = 5
                },
                new Book {
                    Name = "The Hobbit or There and Back Again",
                    AuthorName = "J.R.R. Tolkien",
                    GenreId = 6,
                    DateAdded = DateTime.UtcNow,
                    ReleaseDate = new DateTime(1937, 9, 21),
                    NumberInStock = 10,
                    NumberAvailable = 10
                });

            if (context.Customers.Any())
                return;

            var passwordHasher = new PasswordHasher<Customer>();

            var usersToAdd = new List<Customer> {
                new() {
                    Name = "Jan Kowalski",
                    Email = "jan.kowalski@abc.abc",
                    HasNewsletterSubscribed = true,
                    MembershipTypeId = 2,
                    Birthdate = new DateTime(1970, 7, 5)
                },
                new() {
                    Name = "John Smith",
                    Email = "john.smith@abc.abc",
                    HasNewsletterSubscribed = false,
                    MembershipTypeId = 1,
                    Birthdate = new DateTime(1963, 4, 21)
                }
            };

            foreach (var customer in usersToAdd)
                customer.PasswordHash = passwordHasher.HashPassword(customer, "test1234");

            context.Customers.AddRange(usersToAdd);

            context.SaveChanges();

            if (context.Rentals.Any())
                return;

            context.Rentals.AddRange(
                new Rental {
                    Customer = context.Customers.FirstOrDefault(c => c.Id == 1),
                    Book = context.Books.FirstOrDefault(b => b.Id == 4),
                    DateRented = new DateTime(2021,12,15),
                    DateReturned = new DateTime(2022,01,10)},
                new Rental {
                    Customer = context.Customers.FirstOrDefault(c => c.Id == 2),
                    Book = context.Books.FirstOrDefault(b => b.Id == 2),
                    DateRented = new DateTime(2021, 11, 1)},
                new Rental {
                    Customer = context.Customers.FirstOrDefault(c => c.Id == 1),
                    Book = context.Books.FirstOrDefault(b => b.Id == 3),
                    DateRented = new DateTime(2021, 01, 10)
                });

            context.SaveChanges();
        }
    }
}