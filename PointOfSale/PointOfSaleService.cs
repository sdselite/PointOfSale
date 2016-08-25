using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Domain
{
	public class PointOfSaleService
	{
		private readonly List<Item> scannedItems;
		private readonly ItemService itemService;
		private readonly ReceiptService receiptService;
		private readonly OrderRepository repo;
		private readonly TransactionIdGenerator idGenerator;

		public decimal SubTotal { get { return scannedItems.Sum(i => i.Price); } }
		public IEnumerable<Item> ScannedItems { get { return scannedItems; } }

		public PointOfSaleService(ItemService itemService, ReceiptService receiptService,
								  OrderRepository repo, TransactionIdGenerator generator)
		{
			this.itemService = itemService;
			this.receiptService = receiptService;
			this.repo = repo;
			this.idGenerator = generator;
			this.scannedItems = new List<Item>();
		}

		public void Scan(string barcode)
		{
			itemService.AddItem(barcode, scannedItems);
		}

		public void OnCompleteSale()
		{
			int transactionId = idGenerator.GenerateTransactionId();
			repo.FulFillOrder(transactionId, scannedItems.ToList());
			receiptService.FulFillOrder(transactionId, scannedItems.ToList());
			scannedItems.Clear();
		}
	}

}