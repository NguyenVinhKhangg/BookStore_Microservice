using AutoMapper;

public class BooksImgService : IBooksImgService
{
    private readonly IBooksImgRepository _repo;
    private readonly IMapper _mapper;
    public BooksImgService(IBooksImgRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<BooksImgDto> AddBookImgAsync(BooksImgCreateDto dto)
    {
        var entity = _mapper.Map<BooksImg>(dto);
        var result = await _repo.AddBookImgAsync(entity);
        return _mapper.Map<BooksImgDto>(result);
    }

    public async Task<bool> UpdateBookImgAsync(int imageId, BooksImgUpdateDto dto)
    {
        return await _repo.UpdateBookImgAsync(imageId, dto);
    }

    public async Task<IEnumerable<BooksImgDto>> GetBookImgsByBookIdAsync(int bookId)
    {
        var imgs = await _repo.GetBookImgsByBookIdAsync(bookId);
        return imgs.Select(_mapper.Map<BooksImgDto>);
    }

    public async Task<BooksImgDto?> GetBookImgByIdAsync(int imageId)
    {
        var img = await _repo.GetBookImgByIdAsync(imageId);
        return img == null ? null : _mapper.Map<BooksImgDto>(img);
    }
}
