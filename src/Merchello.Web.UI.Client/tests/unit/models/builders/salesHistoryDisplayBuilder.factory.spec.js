'use strict';
describe('SalesHistoryDisplayBuilder', function() {

    beforeEach(module('umbraco'));

    it('should be able to build an api result', inject(function(salesHistoryDisplayBuilder, dailyAuditLogMock) {
        //// Arrange
        var jsonResult = dailyAuditLogMock.dailyAuditLog();

        //// Act
        var salesHistory = salesHistoryDisplayBuilder.transform(jsonResult);

        //// Assert
        expect(salesHistory).toBeDefined();
        expect(salesHistory.dailyLogs.length).toBeGreaterThan(0);
        expect(salesHistory.dailyLogs[0].day).toBeDefined();
        expect(salesHistory.dailyLogs[0].day).not.toBe('');
        expect(salesHistory.dailyLogs[0].logs[0].entityKey).not.toBe('');

    }));

});