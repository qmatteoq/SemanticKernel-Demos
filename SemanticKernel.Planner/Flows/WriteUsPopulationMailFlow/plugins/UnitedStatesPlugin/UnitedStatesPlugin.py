import requests
from semantic_kernel.skill_definition import (
    sk_function,
    sk_function_context_parameter,
)
from semantic_kernel.orchestration.sk_context import SKContext

class UnitedStatesPlugin:
    @sk_function(
        description="Get the United States population for a specific year",
        name="GetPopulation",
        input_description="The year to get the population for"
    )

    def get_population(self, text: str) -> str:
        url = "https://datausa.io/api/data?drilldowns=Nation&measures=Population"
        response = requests.get(url)
        data = response.json()
        for record in data["data"]:
            if record["Year"] == text:
                result = "The population number in the United States in {year} was {population}".format(year=text, population=record["Population"])
                return result
            
        result = "No data found"
        return result