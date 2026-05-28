using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacao_Web.Data;
using Aplicacao_Web.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Aplicacao_Web.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProdutosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Produtos.Include(p => p.Categoria);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Preco,Estoque,SKU,DataCadastro,CategoriaId")] Produto produto, IFormFile imagemFicheiro)
        {
            if (ModelState.IsValid)
            {
                if (imagemFicheiro != null)
                {
                    string pastaImagens = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");

                    if (!Directory.Exists(pastaImagens))
                        Directory.CreateDirectory(pastaImagens);

                    string nomeFicheiro = Guid.NewGuid().ToString() + "_" + imagemFicheiro.FileName;
                    string caminhoCompleto = Path.Combine(pastaImagens, nomeFicheiro);

                    using (var fileStream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await imagemFicheiro.CopyToAsync(fileStream);
                    }

                    produto.ImagemUrl = nomeFicheiro;
                }

                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Preco,Estoque,SKU,DataCadastro,CategoriaId,ImagemUrl")] Produto produto, IFormFile? imagemFicheiro)
        {
            if (id != produto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagemFicheiro != null)
                    {
                        string pastaImagens = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");

                        // Apagar a imagem antiga para não ocupar espaço
                        if (!string.IsNullOrEmpty(produto.ImagemUrl))
                        {
                            string caminhoAntigo = Path.Combine(pastaImagens, produto.ImagemUrl);
                            if (System.IO.File.Exists(caminhoAntigo)) System.IO.File.Delete(caminhoAntigo);
                        }

                        string nomeFicheiro = Guid.NewGuid().ToString() + "_" + imagemFicheiro.FileName;
                        string caminhoNovo = Path.Combine(pastaImagens, nomeFicheiro);
                        using (var fileStream = new FileStream(caminhoNovo, FileMode.Create))
                        {
                            await imagemFicheiro.CopyToAsync(fileStream);
                        }
                        produto.ImagemUrl = nomeFicheiro;
                    }

                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                // Opcional: Apagar a imagem do disco ao deletar o produto
                if (!string.IsNullOrEmpty(produto.ImagemUrl))
                {
                    string caminho = Path.Combine(_webHostEnvironment.WebRootPath, "imagens", produto.ImagemUrl);
                    if (System.IO.File.Exists(caminho)) System.IO.File.Delete(caminho);
                }

                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}