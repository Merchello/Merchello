'use strict';

/* jasmine specs for controllers go here */

describe('Merchello controllers', function(){

	beforeEach(angular.mock.module('umbraco'));
	
	/* verify that the app is defined */
	it('should have an "umbraco" module defined', function() {
		expect(angular.module('umbraco')).not.toBe(null);
	});

  
	describe("Merchello.PropertyEditors.MerchelloSkuSelector", function(){
	  var scope, ctrl, $httpBackend;
	
	  beforeEach(inject(function(_$httpBackend_, $rootScope, $controller) {
		  $httpBackend = _$httpBackend_;	 
		  scope = $rootScope.$new();
		  ctrl = $controller('MerchelloSkuSelector', {$scope: scope}); 
		}));
		
		
	  /* Goals:
		To check to see if the controller is executed properly.
		To check to see if any important $scope members are set.
		To capture any XHR requests in Unit Testing using mocks.
	  */
	  
	  it('should have a MerchelloSkuSelector controller', function() {
		expect(ctrl).not.toBe(null);
	  });
	  
	  it('should have loaded its assets', function() {
		expect(scope.assetsloaded).toBe(true);
	  });
	  
	});
});
