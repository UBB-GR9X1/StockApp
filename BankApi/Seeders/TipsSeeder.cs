namespace BankApi.Seeders
{
    public class TipsSeeder(IConfiguration configuration) : RegularTableSeeder(configuration)
    {
        protected override string GetQuery() => @"
            IF NOT EXISTS (SELECT 1 FROM Tips) 
            BEGIN
                INSERT INTO Tips 
                    (CreditScoreBracket, Type, TipText)
                VALUES
                ('0-600', 'Credit Improvement', 'Consider paying down outstanding debt to improve your credit score.'),
                ('0-600', 'Responsible Borrowing', 'Maintain timely payments and avoid maxing out credit cards.'),
                ('0-600', 'Budgeting Basics', 'Create a monthly budget to track and control your expenses.'),
                ('0-600', 'Debt Management', 'Focus on paying off high-interest debts first.'),
                ('0-600', 'Credit Monitoring', 'Regularly check your credit report for errors or discrepancies.'),
                ('0-600', 'Emergency Fund', 'Start building an emergency fund to avoid relying on credit.'),
                ('0-600', 'Secured Credit Card', 'Consider using a secured credit card to rebuild your credit.'),
                ('0-600', 'Financial Education', 'Take advantage of free financial literacy resources to improve your knowledge.'),

                ('600-700', 'Financial Growth', 'You qualify for better loan rates. Consider investing wisely.'),
                ('600-700', 'Optimized Credit Usage', 'Your credit score is strong. Keep it that way by diversifying credit types.'),
                ('600-700', 'Savings Strategy', 'Increase your savings rate to prepare for future financial goals.'),
                ('600-700', 'Debt Consolidation', 'Consider consolidating debts to simplify payments and reduce interest.'),
                ('600-700', 'Credit Limit Increase', 'Request a credit limit increase to improve your credit utilization ratio.'),
                ('600-700', 'Retirement Planning', 'Start or increase contributions to your retirement accounts.'),
                ('600-700', 'Insurance Review', 'Review your insurance policies to ensure adequate coverage.'),
                ('600-700', 'Side Income', 'Explore side income opportunities to boost your financial stability.'),

                ('700-850', 'Elite Financial Strategy', 'Your score is excellent! Utilize rewards programs and negotiate better rates.'),
                ('700-850', 'Investment Opportunities', 'Explore advanced investment options like real estate or stocks.'),
                ('700-850', 'Credit Card Rewards', 'Maximize credit card rewards by using them strategically.'),
                ('700-850', 'Wealth Management', 'Consider consulting a financial advisor for wealth management.'),
                ('700-850', 'Estate Planning', 'Start planning your estate to secure your financial legacy.'),
                ('700-850', 'Tax Optimization', 'Work with a tax professional to optimize your tax strategy.'),
                ('700-850', 'Charitable Giving', 'Leverage your financial position to support charitable causes.'),
                ('700-850', 'Diversified Portfolio', 'Ensure your investment portfolio is well-diversified to minimize risk.')
            END;
        ";
    }
}