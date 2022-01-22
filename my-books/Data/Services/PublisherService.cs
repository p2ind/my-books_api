using my_books.Data.Models;
using my_books.Data.Pagging;
using my_books.Data.ViewModels;
using my_books.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace my_books.Data.Services
{
    public class PublisherService
    {
        private AppDbContext _context;
        public PublisherService(AppDbContext context)
        {
            _context = context;
        }

        public Publisher AddPublisher(PublisherVM publisher)
        {
            if (StringStartWithNumber(publisher.Name))
            {
                throw new PublisherNameException("Name starts with number", publisher.Name);
            }
            var _publisher = new Publisher()
            {
                Name = publisher.Name
            };
            _context.Publishers.Add(_publisher);
            _context.SaveChanges();

            return _publisher;
        }

        public List<Publisher> GetAllPublishers(string sortBy, string searchString, int? pageNumber)
        {
            var allPublishers = _context.Publishers.OrderBy(x => x.Name).ToList();

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        allPublishers = allPublishers.OrderByDescending(x => x.Name).ToList();
                        break;
                    default:
                        break;
                }
            }

            if(!string.IsNullOrEmpty(searchString))
            {
                allPublishers = _context.Publishers.Where(x => x.Name.Contains(searchString)).ToList();
            }

            //Pagging
            int pageSize = 5;
            allPublishers = PaginatedList<Publisher>.Create(allPublishers.AsQueryable(), pageNumber ??1,pageSize);

            return allPublishers;
        }

        public Publisher GetPublisherById(int Id) => _context.Publishers.FirstOrDefault(x => x.Id == Id);
        public PublisherWithBookAndAuthorsVM GetPublisherData(int publisherId)
        {
            var _publisherData = _context.Publishers.Where(x => x.Id == publisherId)
                .Select(x => new PublisherWithBookAndAuthorsVM()
                {
                    Name = x.Name,
                    BookAuthors = x.Books.Select(x => new BookAuthorVM()
                    {
                        BookName = x.Title,
                        BookAuthors = x.Book_Authors.Select(x => x.Author.FullName).ToList()
                    }).ToList()
                }).FirstOrDefault();

            return _publisherData;
        }
        public void DeletePublisherById(int id)
        {
            var _publisher = _context.Publishers.FirstOrDefault(x => x.Id == id);
            if(_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"The publisher with id : {id} does not exist");
            }
        }
        private bool StringStartWithNumber(string name) => (Regex.IsMatch(name, @"^\d"));
    }
}
