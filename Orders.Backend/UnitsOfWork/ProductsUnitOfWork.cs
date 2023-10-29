using Orders.Backend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public class ProductsUnitOfWork : GenericUnitOfWork<Product>, IProductsUnitOfWork
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsUnitOfWork(IGenericRepository<Product> repository, IProductsRepository productsRepository) : base(repository)
        {
            _productsRepository = productsRepository;
        }

        public override async Task<Response<IEnumerable<Product>>> GetAsync(PaginationDTO pagination) => await _productsRepository.GetAsync(pagination);

        public override async Task<Response<int>> GetTotalPagesAsync(PaginationDTO pagination) => await _productsRepository.GetTotalPagesAsync(pagination);

        public override async Task<Response<Product>> GetAsync(int id) => await _productsRepository.GetAsync(id);

        public async Task<Response<Product>> AddFullAsync(ProductDTO productDTO) => await _productsRepository.AddFullAsync(productDTO);

        public async Task<Response<Product>> UpdateFullAsync(ProductDTO productDTO) => await _productsRepository.UpdateFullAsync(productDTO);

        public async Task<Response<ImageDTO>> AddImageAsync(ImageDTO imageDTO) => await _productsRepository.AddImageAsync(imageDTO);

        public async Task<Response<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO) => await _productsRepository.RemoveLastImageAsync(imageDTO);
    }
}