
    function ShipmentModelBuilder() {
        // call the super constructor
        ModelBuilder.apply(this, arguments);
    }

    ShipmentModelBuilder.prototype = new ModelBuilder(modelTransformer);