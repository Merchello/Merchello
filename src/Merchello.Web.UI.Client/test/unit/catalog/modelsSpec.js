'use strict';

/* jasmine specs for models go here */

describe("Mechello Catalog Models", function () {
    var $scope;

    beforeEach(inject(function ($rootScope) {
        $scope = $rootScope.$new();
    }));

    describe("Product.Models.ProductAttribute", function () {
        it("Should create an empty product attribute if there is not productAttributeFromServer", function () {
            var productatt = new merchello.Models.ProductAttribute();
            expect(productatt.key).toBe("");
        });
        it("Should create an populated product attribute if there is a productAttributeFromServer", function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var productatt = new merchello.Models.ProductAttribute(pafs);
            expect(productatt.key).toBe("123");
            expect(productatt.optionKey).toBe("321");
            expect(productatt.name).toBe("bane");
            expect(productatt.sortOrder).toBe(6);
            expect(productatt.optionOrder).toBe(6);       //Possible Issue: Needs Review
            expect(productatt.isRemoved).toBe(false);   //Possible Issue: Needs Review
        });

        it("Should return the correct option out of a list of options.", function () {
            var options = [];
            for (var i in [1, 2, 3, 4, 5, 6, 7]) {
                var pofs = { key: i.toString(), name: "bane", required: "aye", sortOrder: 6 };
                //var productopt = new merchello.Models.ProductOption(pofs);
                options.push(pofs);
            }
            var pafs = { key: "123", optionKey: "6", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var productatt = new merchello.Models.ProductAttribute(pafs);
            var option = productatt.findMyOption(options);
            expect(option).not.toBeUndefined();
            expect(option.key).toBe("6");
        });
    });

    describe("Product.Models.ProductOption", function () {
        var productopt;
        var pofs;

        beforeEach(function () {
            pofs = { key: "123", name: "bane", required: "aye", sortOrder: 6 };
            productopt = new merchello.Models.ProductOption(pofs);
        });

        it("Should create an empty product option if there is not productOptionFromServer", function () {
            productopt = new merchello.Models.ProductOption();
            expect(productopt.key).toBe("");
            expect(productopt.required).toBe("");
            expect(productopt.name).toBe("");
            expect(productopt.sortOrder).toBe(1);
            expect(productopt.choices.length).toBe(0);
        });
        it("Should create an populated product option if there is a productOptionFromServer", function () {
            expect(productopt.key).toBe("123");
            expect(productopt.required).toBe("aye");
            expect(productopt.name).toBe("bane");
            expect(productopt.sortOrder).toBe(6);
            expect(productopt.choices.length).toBe(0);
        });

        it("Should add a new choice", function () {
            productopt.addChoice("plane");
            expect(productopt.choices.length).toBe(1);
            expect(productopt.choices[0].sortOrder).toBe(productopt.choices.length);
            expect(productopt.choices[0].optionKey).toBe(productopt.key);
        });

        it("Should remove a choice", function () {
            productopt.addChoice("plane");
            var choice = productopt.removeChoice(0);
            expect(choice).not.toBeUndefined();
            expect(productopt.choices.length).toBe(0);
        });

        it("Should make the sortOrder sync with the order in the choices array", function () {
            productopt.addChoice("plane");
            productopt.choices[0].sortOrder += 3;
            productopt.addChoice("mane");
            productopt.choices[1].sortOrder += 2;
            productopt.addChoice("dane");
            productopt.choices[1].sortOrder += 1;

            productopt.resetChoiceSortOrder();
            for (var i = 0; i < productopt.choices.length; i++) {
                expect(productopt.choices[i].sortOrder).toBe(i + 1);
            }

        });

        it("Should check if this choice exists in my choices array", function () {
            productopt.addChoice("plane");
            productopt.choices[0].key = "321"
            var choice = productopt.choices[0];
            productopt.addChoice("fame");
            productopt.choices[1].key = "123"
            var exists = productopt.choiceExists(choice);
            expect(exists).toBe(true);
        });

        it("Should find a choice by name and return it or undefined if not found", function () {
            productopt.addChoice("plane");
            productopt.choices[0].key = "321"
            productopt.addChoice("fame");
            productopt.choices[1].key = "123"
            var choice = productopt.findChoiceByName("plane");
            expect(choice).not.toBeUndefined();
            choice = productopt.findChoiceByName("main");
            expect(choice).toBe(undefined);
        });
    });

    describe("Product.Models.ProductVariant", function () {
        var productvar;
        var pvfs;

        beforeEach(function () {
            var pofs = { key: "123", name: "bane", required: "aye", sortOrder: 6 };
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            pvfs = {
                key: "123",
                name: "bane",
                productKey: "asdfg",
                sku: "1qa2ws",
                price: 1.00,
                costOfGoods: 2.00,
                salePrice: 3.00,
                onSale: true,
                manufacturer: "Me",
                manufacturerModelNumber: "1234567890",
                weight: 10,
                length: 11,
                width: 12,
                height: 13,
                barcode: "10101010",
                available: false,
                trackInventory: true,
                outOfStockPurchase: true,
                taxable: true,
                shippable: true,
                download: true,
                downloadMediaId: 123890,
                totalInventoryCount: 1000000000,
                attributes: [pafs, pafs],
                attributeKeys: ["123"],
                catalogInventories: []
            };

            productvar = new merchello.Models.ProductVariant(pvfs);
        });

        it("should create an empty product variant", function () {
            productvar = new merchello.Models.ProductVariant();
            expect(productvar).not.toBeUndefined();
            expect(productvar.attributes.length).toBe(0);
            expect(productvar.selected).toBe(false);
            expect(productvar.key).toBe("");
            expect(productvar.name).toBe("");
        });

        it("should create an filled product variant", function () {
            expect(productvar).not.toBeUndefined();
            expect(productvar.attributes.length).toBe(2);
            expect(productvar.catalogInventories.length).toBe(0);
            expect(productvar.selected).toBe(false);
            expect(productvar.key).toBe("123");
            expect(productvar.name).toBe("bane");
        });

        it("should copy from product to create a master variant", function () {
            expect(productvar).not.toBeUndefined();
            var p = { key: "234", name: "same", catalogInventories: [] };
            productvar.copyFromProduct(p);
            expect(productvar.attributes.length).toBe(0);
            expect(productvar.catalogInventories.length).toBe(0);
            expect(productvar.selected).toBe(false);
            expect(productvar.key).toBe("123");
            expect(productvar.productKey).toBe("234");
            expect(productvar.name).toBe("same");
        });

        it("should fix attribute sort orders", function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var pofs = { key: "123", name: "bane", required: "aye", sortOrder: 6 };
            var options = [];
            for (var i in [1, 2, 3, 4]) {
                pofs.sortorder = i + 1;
                pafs.optionorder = 4 - i + 1;
                pafs.key += i.toString();
                pafs.name += i.toString();
                options.push(new merchello.Models.ProductOption(pofs));
                productvar.attributes.push(new merchello.Models.ProductAttribute(pafs));
            }
            productvar.fixAttributeSortOrders()
            for (var i in productvar.attributes) {
                expect(i.optionOrder).toBe(i.sortOrder);
            }
        });

        it("should add Attributes As Properties for sorting in a table", function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var pofs = { key: "123", name: "bane", required: "aye", sortOrder: 6 };
            var options = [];
            productvar = new merchello.Models.ProductVariant();

            for (var i in [1, 2, 3, 4]) {
                pofs.sortorder = i + 1;
                pafs.optionorder = 4 - i + 1;
                pofs.key = i.toString();
                pofs.name += i.toString();

                pafs.optionKey = i.toString();
                pafs.name += i.toString();
                options.push(new merchello.Models.ProductOption(pofs));
                productvar.attributes.push(new merchello.Models.ProductAttribute(pafs));
            }
            productvar.addAttributesAsProperties(options);

            _.each(options, function (i) {
                expect(productvar[i.name]).not.toBeUndefined();
            });
        });


        it("should return if it isComposedFromAttribute", function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            productvar.attributes.push(pafs);
            expect(productvar.isComposedFromAttribute(pafs)).toBe(true);
            var new_pafs = _.extend({}, pafs);
            new_pafs.key = "###";
            expect(productvar.isComposedFromAttribute(new_pafs)).toBe(false);
        });

        it("should ensure the catalog inventory", function () {
            var warehouse = { key: "meh", warehouseCatalogs: [{ key: "123", name: "Matt" }] };
            expect(productvar.catalogInventories.length).toBe(0);
            productvar.ensureCatalogInventory(warehouse);
            expect(productvar.catalogInventories.length).toBe(1);
            productvar.ensureCatalogInventory(warehouse);
            expect(productvar.catalogInventories.length).toBe(1);
        });

        it("should add a variant to this product", function () {
            var warehouse = { key: "meh", warehouseCatalogs: [{ key: "123", name: "Matt" }] };
            productvar.addCatalogInventory(warehouse);
            expect(productvar.catalogInventories.length).toBe(1);
            expect(productvar.catalogInventories[0].productVariantKey).toBe(productvar.key);
            expect(productvar.catalogInventories[0].warehouseKey).toBe(warehouse.key);
            expect(productvar.catalogInventories[0].catalogKey).toBe("123");
            expect(productvar.catalogInventories[0].catalogName).toBe("Matt");
        });

        it("should update the global inventory on change", function () {
            productvar.globalInventoryChanged(5);
            expect(productvar.totalInventoryCount).toBe(productvar.catalogInventories * 5);
            _.each(productvar.catalogInventories, function (i) {
                expect(i.count).toBe(5);
            });
        });
    });

    describe("Product.Models.Product", function () {
        var product;
        var pfs;

        beforeEach(function () {
            var pafs = { key: "123", optionKey: "321", name: "bane", sortOrder: 6, optionOrder: 7, isRemoved: true };
            var pofs = { key: "123", name: "bane", required: "aye", sortOrder: 6, choices: [_.extend({}, pafs), _.extend({}, pafs)] };
            var pofsv2 = { key: "321", name: "fame", required: "neh", sortOrder: 3, choices: [_.extend({}, pafs), _.extend({}, pafs)] };
            var pvfs = {
                key: "123",
                name: "bane",
                productKey: "asdfg",
                sku: "1qa2ws",
                price: 1.00,
                costOfGoods: 2.00,
                salePrice: 3.00,
                onSale: true,
                manufacturer: "Me",
                manufacturerModelNumber: "1234567890",
                weight: 10,
                length: 11,
                width: 12,
                height: 13,
                barcode: "10101010",
                available: false,
                trackInventory: true,
                outOfStockPurchase: true,
                taxable: true,
                shippable: true,
                download: true,
                downloadMediaId: 123890,
                totalInventoryCount: 1000000000,
                attributes: [pafs, pafs],
                attributeKeys: ["123"],
                catalogInventories: []
            };

            pfs = {
                key: "123",
                name: "bane",
                sku: "1qa2ws",
                price: 1.00,
                costOfGoods: 2.00,
                salePrice: 3.00,
                onSale: true,
                manufacturer: "Me",
                manufacturerModelNumber: "1234567890",
                weight: 10,
                length: 11,
                width: 12,
                height: 13,
                barcode: "10101010",
                available: false,
                trackInventory: true,
                outOfStockPurchase: true,
                taxable: true,
                shippable: true,
                download: true,
                downloadMediaId: 123890,
                hasOptions: true,
                hasVariants: true,
                productOptions: [pofs, pofsv2],
                productVariants: [pvfs, (pvfs.sortOrder = 9)]
            };
            product = new merchello.Models.Product(pfs, false);
        });

        it("should create a defined object", function () {
            expect(product).not.toBeUndefined();
            for (var i in pfs) {
                expect(product[i]).not.toBeUndefined();
            }

        });

        it("should create a empty object", function () {
            product = new merchello.Models.Product();

            expect(product.key).toBe("");
            expect(product.name).toBe("");
            expect(product.sku).toBe("");
            expect(product.price).toBe(0.00);
            expect(product.costOfGoods).toBe(0.00);
            expect(product.salePrice).toBe(0.00);
            expect(product.onSale).toBe(false);
            expect(product.manufacturer).toBe("");
            expect(product.manufacturerModelNumber).toBe("");
            expect(product.weight).toBe(0);
            expect(product.length).toBe(0);
            expect(product.width).toBe(0);
            expect(product.height).toBe(0);
            expect(product.barcode).toBe("");
            expect(product.available).toBe(true);
            expect(product.trackInventory).toBe(false);
            expect(product.outOfStockPurchase).toBe(false);
            expect(product.taxable).toBe(false);
            expect(product.shippable).toBe(false);
            expect(product.download).toBe(false);
            expect(product.downloadMediaId).toBe(-1);
            expect(product.hasOptions).toBe(false);
            expect(product.hasVariants).toBe(false);

            expect(product.productOptions.length).toBe(0);

            expect(product.productVariants.length).toBe(0);

            expect(product.catalogInventories.length).toBe(0);
        });

        it("should create a populated product but not map the children", function () {
            product = new merchello.Models.Product(pfs, true);
            expect(product.productOptions).toBe(undefined);
            expect(product.productVariants).toBe(undefined);
        });

        it("should add a blank options to the product", function () {
            product.productOptions = undefined;
            product.hasOptions = false;
            product.addBlankOption();
            expect(product.productOptions).not.toBeUndefined();
            expect(product.productOptions.length).toBe(1);
            expect(product.hasOptions).toBe(true);
            product.productOptions = [];
            product.addBlankOption();
            expect(product.productOptions.length).toBe(1);
            expect(product.productOptions[0].key).toBe("");
        });

        it("should remove an option from it's productOptions", function () {
            expect(product.productOptions.length).toBe(2);
            product.removeOption(product.productOptions[0]);
            expect(product.productOptions.length).toBe(1);
        });

        it("should create an array of all choices in a flattened list", function () {
            var flattenedlist = product.flattened();
            var choices = ["choice1", "choice2"];
            expect(flattenedlist.length).toBe(product.productOptions.length * 2);
            for (var i; i < flattenedlist.length; i++) {
                expect(flattenedlist[i]).toBe(choices[i % 2]);
            }
        });

        it("should add variant to the product", function () {
            var attrs = [{ one: "one", key: "one" }, { two: "two", key: "two" }];
            product.hasVariants = false;
            var vari = product.addVariant(attrs);
            expect(vari.name).toBe("");
            expect(vari.selected).toBe(true);
            expect(vari.sku).toBe("");
            expect(vari.attributeKeys.length).toBe(2);
            expect(product.productVariants.length).toBe(3);
            expect(product.hasVariants).toBe(true);
        });

        it("should remove variant to the product", function () {
            expect(product.productVariants.length).toBe(2);
            product.removeVariant(0);
            expect(product.productVariants.length).toBe(1);
            product.removeVariant(0);
            expect(product.productVariants.length).toBe(0);
        });

        it("should return the lowest variant price if the product has variants", function () {
            for (var i = 0; i < product.productVariants.length; i++) {
                product.productVariants[i].price = i + 1;
            }
            expect(product.variantsMinimumPrice().price).toBe(1);
            product.productVariants = undefined;
            product.minPrice = { price: 1200 };
            expect(product.variantsMinimumPrice().price).toBe(1200);
        });

        it("should return the lowest variant price if the product has variants", function () {
            for (var i = 0; i < product.productVariants.length; i++) {
                product.productVariants[i].price = i + 1;
            }
            expect(product.variantsMaximumPrice().price).toBe(product.productVariants.length);
            product.productVariants = undefined;
            product.maxPrice = { price: 1200 };
            expect(product.variantsMaximumPrice().price).toBe(1200);
        });
    });
});
