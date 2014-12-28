
    // we just need s simple function that takes the model transformer here.
    function ModelBuilder(modelTransformer) {
        this.transformer = modelTransformer;
    }

    // This will be used to create models in most cases but it will not
    // handle object that have child objects
    ModelBuilder.prototype.Build = function(jsonResult, constructor) {
        return this.transformer.transform(jsonResult, constructor);
    };