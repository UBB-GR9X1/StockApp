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
                
                -- Roasts  
                ('Roast', 'Roast', 'Your fees are so high, even your calculator needs a loan to keep up.'),  
                ('Roast', 'Roast', 'You call it a service fee, we call it daylight robbery.'),  
                ('Roast', 'Roast', 'Your interest rates are so high, they should come with an oxygen mask.'),  
                ('Roast', 'Roast', 'Banking with you feels like a subscription to disappointment.'),  
                ('Roast', 'Roast', 'Your overdraft fees are the real reason people need loans.'),  
                ('Roast', 'Roast', 'Your customer service is so slow, it’s practically a savings plan.'),  
                ('Roast', 'Roast', 'Your app crashes more often than my financial dreams.'),  
                ('Roast', 'Roast', 'Your loan approval process is slower than a dial-up connection.'),  
                ('Roast', 'Roast', 'Your hidden fees are so well-hidden, even Sherlock Holmes couldn’t find them.'),  
                ('Roast', 'Roast', 'Your savings account interest is so low, it’s practically a rounding error.'),  
                ('Roast', 'Roast', 'Your credit card rewards are as rewarding as a participation trophy.'),  
                ('Roast', 'Roast', 'Your mortgage terms are so confusing, they should come with a decoder ring.'),  
                ('Roast', 'Roast', 'Your ATM fees are the reason people carry cash again.'),  
                ('Roast', 'Roast', 'Your financial advice is as useful as a screen door on a submarine.'),  
                ('Roast', 'Roast', 'Your fraud protection is so weak, even a toddler could hack it.'),  
                ('Roast', 'Roast', 'Your account maintenance fees are the real reason people go off the grid.'),  
                ('Roast', 'Roast', 'Your loan officers are so stingy, they probably charge their own kids interest.'),  
                ('Roast', 'Roast', 'Your credit score system is as transparent as a brick wall.'),  
                ('Roast', 'Roast', 'Your financial planning tools are as helpful as a map with no directions.'),  
                ('Roast', 'Roast', 'Your bank statements are so complicated, they should come with a PhD in accounting.'),  

                -- Punishments  
                ('Punishment', 'Punishment', 'For every overdraft, you must write a 500-word essay on financial responsibility.'),  
                ('Punishment', 'Punishment', 'Every declined loan application comes with a mandatory budgeting workshop.'),  
                ('Punishment', 'Punishment', 'Late payments result in a public reading of your financial mistakes.'),  
                ('Punishment', 'Punishment', 'Miss a payment? You owe us a handwritten apology letter.'),  
                ('Punishment', 'Punishment', 'For every bounced check, you must donate to a financial literacy charity.'),  
                ('Punishment', 'Punishment', 'Overdraft fees now come with a mandatory financial counseling session.'),  
                ('Punishment', 'Punishment', 'Late fees include a requirement to attend a debt management seminar.'),  
                ('Punishment', 'Punishment', 'Every credit card swipe over your limit triggers a motivational speech from your banker.'),  
                ('Punishment', 'Punishment', 'Miss a mortgage payment? You owe us a poem about fiscal responsibility.'),  
                ('Punishment', 'Punishment', 'For every late fee, you must explain your spending habits to a panel of financial experts.'),  
                ('Punishment', 'Punishment', 'Every overdraft triggers a mandatory viewing of a budgeting documentary.'),  
                ('Punishment', 'Punishment', 'Miss a payment? You must volunteer at a financial literacy workshop.'),  
                ('Punishment', 'Punishment', 'For every declined transaction, you owe us a financial plan for the next month.'),  
                ('Punishment', 'Punishment', 'Every bounced check comes with a mandatory savings account deposit.'),  
                ('Punishment', 'Punishment', 'Late payments now include a requirement to read a book on personal finance.'),  
                ('Punishment', 'Punishment', 'For every overdraft, you must create a detailed monthly budget and submit it.'),  
                ('Punishment', 'Punishment', 'Miss a payment? You owe us a financial goals presentation.'),  
                ('Punishment', 'Punishment', 'Every late fee triggers a mandatory meeting with a financial advisor.'),  
                ('Punishment', 'Punishment', 'For every declined loan, you must write a letter explaining why you need it.'),  
                ('Punishment', 'Punishment', 'Miss a payment? You owe us a financial literacy quiz with a passing score.'),  

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